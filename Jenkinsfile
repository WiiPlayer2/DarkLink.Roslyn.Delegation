def runStage()
{
    def configuredBuild = load "ci/jenkins/configuredBuild.groovy"
    configuredBuild.run(STAGE_NAME)
}

def createStage(stageName)
{
    stage(stageName)
    {
        steps
        {
            script
            {
                runStage();
            }
        }
    }
}

pipeline
{
    agent any
    stages
    {
        createStage('Build');
    }
}
