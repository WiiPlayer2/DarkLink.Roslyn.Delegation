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
        stage('Build')
        {
            steps
            {
                script
                {
                    runStage()
                }
            }
        }
    }
}
