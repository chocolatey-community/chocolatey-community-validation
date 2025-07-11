import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.script
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.pullRequests
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.nuGetPublish
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.ScheduleTrigger
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.schedule
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.GitVcsRoot

project {
    buildType(ChocolateyCommunityValidation)
    buildType(ChocolateyCommunityValidationSchd)
    buildType(ChocolateyCommunityValidationQA)
}

object ChocolateyCommunityValidation : BuildType({
    id = AbsoluteId("ChocolateyCommunityValidation")
    name = "Build (Unit Tests)"

    templates(AbsoluteId("SlackNotificationTemplate"))

    artifactRules = """
    """.trimIndent()

    params {
        password("env.EAZFUSCATOR_NET_LICENSE", "credentialsJSON:cdac4820-7cd7-54e4-b539-8ecaa3490a3c", display = ParameterDisplay.HIDDEN, readOnly = true)
        param("env.vcsroot.branch", "%vcsroot.branch%")
        param("env.Git_Branch", "%teamcity.build.vcs.branch.ChocolateyCommunityValidation_CommunityValVcsRoot%")
        param("teamcity.git.fetchAllHeads", "true")
        password("env.GITHUB_PAT", "%system.GitHubPAT%", display = ParameterDisplay.HIDDEN, readOnly = true)
    }

    vcs {
        root(DslContext.settingsRoot)

        branchFilter = """
            +:*
        """.trimIndent()
    }

    steps {
        step {
            name = "Include Signing Keys"
            type = "PrepareSigningEnvironment"
        }

        script {
            name = "Call Cake"
            scriptContent = """
                build.official.bat --verbosity=diagnostic --target=CI --testExecutionType=unit --shouldRunOpenCover=false
            """.trimIndent()
        }
    }

    triggers {
        vcs {
            branchFilter = """
                +:*
                -:master
                -:support/*
            """.trimIndent()
        }
    }

    features {
        pullRequests {
            provider = github {
                authType = token {
                    token = "%system.GitHubPAT%"
                }
            }
        }
    }
})

object ChocolateyCommunityValidationSchd : BuildType({
    id = AbsoluteId("ChocolateyCommunityValidationSchd")
    name = "Build (Scheduled Integration Testing)"

    templates(AbsoluteId("SlackNotificationTemplate"))

    artifactRules = """
    """.trimIndent()

    params {
        password("env.EAZFUSCATOR_NET_LICENSE", "credentialsJSON:cdac4820-7cd7-54e4-b539-8ecaa3490a3c", display = ParameterDisplay.HIDDEN, readOnly = true)
        param("env.vcsroot.branch", "%vcsroot.branch%")
        param("env.Git_Branch", "%teamcity.build.vcs.branch.ChocolateyCommunityValidation_CommunityValVcsRoot%")
        param("teamcity.git.fetchAllHeads", "true")
        password("env.GITHUB_PAT", "%system.GitHubPAT%", display = ParameterDisplay.HIDDEN, readOnly = true)
    }

    vcs {
        root(DslContext.settingsRoot)

        branchFilter = """
            +:*
        """.trimIndent()
    }

    steps {
        script {
            name = "Call Cake"
            scriptContent = """
                build.official.bat --verbosity=diagnostic --target=CI --testExecutionType=all --shouldRunOpenCover=false --shouldRunAnalyze=false --shouldRunIlMerge=false --shouldObfuscateOutputAssemblies=false --shouldRunChocolatey=false --shouldRunNuGet=false --shouldAuthenticodeSignOutputAssemblies=false --shouldAuthenticodeSignPowerShellScripts=false
            """.trimIndent()
        }
    }

    triggers {
        schedule {
            schedulingPolicy = daily {
                hour = 2
                minute = 0
            }
            branchFilter = """
                +:<default>
            """.trimIndent()
            triggerBuild = always()
            withPendingChangesOnly = false
        }
    }
})

object ChocolateyCommunityValidationQA : BuildType({
    id = AbsoluteId("ChocolateyCommunityValidationQA")
    name = "Build (SonarQube)"

    templates(AbsoluteId("SlackNotificationTemplate"))

    artifactRules = """
    """.trimIndent()

    params {
        password("env.EAZFUSCATOR_NET_LICENSE", "credentialsJSON:cdac4820-7cd7-54e4-b539-8ecaa3490a3c", display = ParameterDisplay.HIDDEN, readOnly = true)
        param("env.vcsroot.branch", "%vcsroot.branch%")
        param("env.Git_Branch", "%teamcity.build.vcs.branch.ChocolateyCommunityValidation_CommunityValVcsRoot%")
        param("env.SONARQUBE_ID", "chocolatey-community-validation")
        param("teamcity.git.fetchAllHeads", "true")
        password("env.GITHUB_PAT", "%system.GitHubPAT%", display = ParameterDisplay.HIDDEN, readOnly = true)
    }

    vcs {
        root(DslContext.settingsRoot)

        branchFilter = """
            +:*
        """.trimIndent()
    }

    steps {
        script {
            name = "Call Cake"
            scriptContent = """
                build.official.bat --verbosity=diagnostic --target=CI --testExecutionType=none --shouldRunSonarQube=true --shouldRunDependencyCheck=true --shouldRunOpenCover=false --shouldRunAnalyze=false --shouldRunIlMerge=false --shouldObfuscateOutputAssemblies=false --shouldRunChocolatey=false --shouldRunNuGet=false --shouldAuthenticodeSignOutputAssemblies=false --shouldAuthenticodeSignPowerShellScripts=false
            """.trimIndent()
        }
    }

    triggers {
        schedule {
            schedulingPolicy = weekly {
                dayOfWeek = ScheduleTrigger.DAY.Saturday
                hour = 2
                minute = 45
            }
            branchFilter = """
                +:<default>
            """.trimIndent()
            triggerBuild = always()
            withPendingChangesOnly = false
        }
    }
})
