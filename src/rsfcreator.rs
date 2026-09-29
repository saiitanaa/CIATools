use std::fs;
use std::io;
use std::path::PathBuf;

#[allow(non_camel_case_types)]
#[derive(Debug, Default)]
pub struct rsf_config {
    pub title: String,
    pub companyCode: String,
    pub productCode: String,
    pub romFsPath: String,
    pub uniqueId: String,
    pub saveDataSize: String,
    pub cpuSpeed: String, // 804Mhz New 3DS, 268MHz Old 3DS
}

impl rsf_config {
    pub fn generate(&self) -> String {
        format!(
            r#"BasicInfo:
  Title                    : "{title}"
  CompanyCode              : "{companyCode}"
  ProductCode              : "{productCode}"
  ContentType              : Application
  Logo                     : Homebrew

RomFs:
  RootPath                 : "{romFsPath}"

TitleInfo:
  UniqueId                 : {uniqueId}
  Category                 : Application

CardInfo:
  MediaSize                : 128MB
  MediaType                : Card1
  CardDevice               : None

Option:
  UseOnSD                  : true
  FreeProductCode          : true
  MediaFootPadding         : false
  EnableCrypt              : false
  EnableCompress           : true

SystemControlInfo:
  SaveDataSize: {saveDataSize}
  RemasterVersion: 0
  StackSize: 0x40000

# DO NOT EDIT BELOW HERE OR PROGRAMS WILL NOT LAUNCH (most likely)

AccessControlInfo:
  FileSystemAccess:
   - Debug
   - DirectSdmc
   - DirectSdmcWrite

  IdealProcessor                : 0
  AffinityMask                  : 1
  Priority                      : 16

  MaxCpu                        : 0x9E
  DisableDebug                  : false
  EnableForceDebug              : false
  CanWriteSharedPage            : false
  CanUsePrivilegedPriority      : false
  CanUseNonAlphabetAndNumber    : false
  PermitMainFunctionArgument    : false
  CanShareDeviceMemory          : false
  RunnableOnSleep               : false
  SpecialMemoryArrange          : false
  CoreVersion                   : 2
  DescVersion                   : 2

  ReleaseKernelMajor            : "02"
  ReleaseKernelMinor            : "33"
  MemoryType                    : Application
  HandleTableSize: 512

  SystemModeExt                 : Legacy
  CpuSpeed                      : {cpuSpeed}
  EnableL2Cache                 : true
  CanAccessCore2                : true

  IORegisterMapping:
   - 1ff50000-1ff57fff
   - 1ff70000-1ff77fff
  MemoryMapping:
   - 1f000000-1f5fffff:r

  SystemCallAccess:
     ArbitrateAddress: 34
     Break: 60
     CancelTimer: 28
     ClearEvent: 25
     ClearTimer: 29
     CloseHandle: 35
     ConnectToPort: 45
     ControlMemory: 1
     CreateAddressArbiter: 33
     CreateEvent: 23
     CreateMemoryBlock: 30
     CreateMutex: 19
     CreateSemaphore: 21
     CreateThread: 8
     CreateTimer: 26
     DuplicateHandle: 39
     ExitProcess: 3
     ExitThread: 9
     GetCurrentProcessorNumber: 17
     GetHandleInfo: 41
     GetProcessId: 53
     GetProcessIdOfThread: 54
     GetProcessIdealProcessor: 6
     GetProcessInfo: 43
     GetResourceLimit: 56
     GetResourceLimitCurrentValues: 58
     GetResourceLimitLimitValues: 57
     GetSystemInfo: 42
     GetSystemTick: 40
     GetThreadContext: 59
     GetThreadId: 55
     GetThreadIdealProcessor: 15
     GetThreadInfo: 44
     GetThreadPriority: 11
     MapMemoryBlock: 31
     OutputDebugString: 61
     QueryMemory: 2
     ReleaseMutex: 20
     ReleaseSemaphore: 22
     SendSyncRequest1: 46
     SendSyncRequest2: 47
     SendSyncRequest3: 48
     SendSyncRequest4: 49
     SendSyncRequest: 50
     SetThreadPriority: 12
     SetTimer: 27
     SignalEvent: 24
     SleepThread: 10
     UnmapMemoryBlock: 32
     WaitSynchronization1: 36
     WaitSynchronizationN: 37

  InterruptNumbers:

  ServiceAccessControl:
   - APT:U
   - $hioFIO
   - $hostio0
   - $hostio1
   - ac:u
   - boss:U
   - cam:u
   - ir:rst
   - cfg:u
   - dlp:FKCL
   - dlp:SRVR
   - dsp::DSP
   - frd:u
   - fs:USER
   - gsp::Gpu
   - hid:USER
   - http:C
   - mic:u
   - ndm:u
   - news:s
   - nwm::UDS
   - ptm:u
   - pxi:dev
   - soc:U
   - gsp::Lcd
   - y2r:u
   - ldr:ro
   - ir:USER
   - ir:u
   - csnd:SND
   - am:u
   - ns:s

SystemControlInfo:
  Dependency:
    ac: 0x0004013000002402L
    am: 0x0004013000001502L
    boss: 0x0004013000003402L
    camera: 0x0004013000001602L
    cecd: 0x0004013000002602L
    cfg: 0x0004013000001702L
    codec: 0x0004013000001802L
    csnd: 0x0004013000002702L
    dlp: 0x0004013000002802L
    dsp: 0x0004013000001a02L
    friends: 0x0004013000003202L
    gpio: 0x0004013000001b02L
    gsp: 0x0004013000001c02L
    hid: 0x0004013000001d02L
    http: 0x0004013000002902L
    i2c: 0x0004013000001e02L
    ir: 0x0004013000003302L
    mcu: 0x0004013000001f02L
    mic: 0x0004013000002002L
    ndm: 0x0004013000002b02L
    news: 0x0004013000003502L
    nim: 0x0004013000002c02L
    nwm: 0x0004013000002d02L
    pdn: 0x0004013000002102L
    ps: 0x0004013000003102L
    ptm: 0x0004013000002202L
    ro: 0x0004013000003702L
    socket: 0x0004013000002e02L
    spi: 0x0004013000002302L
    ssl: 0x0004013000002f02L
"#,
            title = self.title,
            companyCode = self.companyCode,
            productCode = self.productCode,
            romFsPath = self.romFsPath,
            uniqueId = self.uniqueId,
            saveDataSize = self.saveDataSize,
            cpuSpeed = self.cpuSpeed,
        )
    }
}

pub fn create_rsf(config: &rsf_config) -> io::Result<()> {
    let user_files = std::env::current_exe()?
        .parent()
        .ok_or_else(|| io::Error::other("Binary path error"))?
        .join("DATA")
        .join("USER_FILES");

    fs::create_dir_all(&user_files)?;

    let rsf_path: PathBuf = user_files.join("config.rsf");

    let content = config.generate();

    fs::write(rsf_path, content)?;

    Ok(())
}
