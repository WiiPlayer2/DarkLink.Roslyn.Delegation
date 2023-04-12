def runStage()
{
    def configuredBuild = load "ci/jenkins/configuredBuild.groovy";
    configuredBuild.runAll();
}

pipeline
{
    agent {
        docker { image 'mcr.microsoft.com/dotnet/sdk:6.0' }
    }

    stages { stage('') { steps { script { runStages(); } } } }
}
