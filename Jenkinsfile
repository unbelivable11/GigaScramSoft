node {
    stage('Checkout') {
        cleanWs()
        checkout scm
    }

    stage('Build') {
        bat dotnet.exe restore \"${workspace}\\GigaScramSoft.sln\""
        bat dotnet.exe build \"${workspace}\\GigaScramSoft.sln\""
    }

    stage('UnitTests') {
        catchError(buildResult: 'UNSTABLE', stageResult: 'FAILURE') {
            def testStatus = bat returnStatus: true, script: dotnet.exe test \"${workspace}\\GigaScramSoft.sln\" --logger \"trx;LogFileName=unit_tests.xml\" --no-build"
            step([$class: 'MSTestPublisher', testResultsFile: "**/unit_tests.xml", failOnError: true, keepLongStdio: true])
        }
    }
}