$RESULTS_WORKSPACE = "$(Get-Location)\load-test\user-files\results"
$GATLING_BIN_DIR = "$env:GATLING_HOME\bin"
$GATLING_WORKSPACE = "$(Get-Location)\load-test\user-files"


function Run-Gatling {
    & "$env:JAVA_HOME\bin\java.exe" -Xms32M -Xmx128M -cp "$env:GATLING_HOME\lib\*" io.gatling.bundle.GatlingCLI -rm local -s RinhaBackendCrebitosSimulation -rf $RESULTS_WORKSPACE -sf "$GATLING_WORKSPACE\simulations"
}

function Start-Test {
    for ($i = 1; $i -le 20; $i++) {
        Write-Output $i
        try {
            Run-Gatling
            break
        } catch {
            Start-Sleep -Seconds 2
            Write-host "Encontrado erro:" $_
        }
    }
}

Start-Test