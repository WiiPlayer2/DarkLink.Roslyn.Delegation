def runStage()
{
    def configuredBuild = load "ci/jenkins/configuredBuild.groovy"
    configuredBuild.run(STAGE_NAME)
}

pipeline
{
    agent any
    stages
    {
        stage('Cleanup') { steps { script { runStage(); } } }
        stage('Build') { steps { script { runStage(); } } }
        stage('Test') { steps { script { runStage(); } } }
        stage('Pack') { steps { script { runStage(); } } }
        stage('Publish') { steps { script { runStage(); } } }
    }
}
