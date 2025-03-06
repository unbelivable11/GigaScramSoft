node {
    stage('Checkout') {
        cleanWs()
        checkout scm
    }

    stage('Build') {
        bat "dir /B"  // Проверим, где находится .sln
        echo "${workspace}\\GigaScramSoft.sln"

        bat "\"C:/Program Files/dotnet/dotnet.exe\" restore \"${workspace}\\GigaScramSoft.sln\""
        bat "\"C:/Program Files/dotnet/dotnet.exe\" build \"${workspace}\\GigaScramSoft.sln\""
    }

    stage('UnitTests') {
        catchError(buildResult: 'UNSTABLE', stageResult: 'FAILURE') {
            def testStatus = bat returnStatus: true, script: "\"C:/Program Files/dotnet/dotnet.exe\" test \"${workspace}\\GigaScramSoft.sln\" --logger \"trx;LogFileName=unit_tests.xml\" --no-build"
            step([$class: 'MSTestPublisher', testResultsFile: "**/unit_tests.xml", failOnError: true, keepLongStdio: true])
        }
    }
}
