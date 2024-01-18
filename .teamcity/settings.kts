import jetbrains.buildServer.configs.kotlin.v2019_2.*
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.script
import jetbrains.buildServer.configs.kotlin.v2019_2.buildSteps.powerShell
import jetbrains.buildServer.configs.kotlin.v2019_2.buildFeatures.pullRequests
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.vcs
import jetbrains.buildServer.configs.kotlin.v2019_2.triggers.schedule
import jetbrains.buildServer.configs.kotlin.v2019_2.vcs.GitVcsRoot

project {
    buildType(ChocolateyCommunityValidation)
}

object ChocolateyCommunityValidation : BuildType({
    id = AbsoluteId("ChocolateyCommunityValidation")
    name = "Build"

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
                IF "%teamcity.build.triggeredBy%" == "Schedule Trigger" (SET TestType=all) ELSE (SET TestType=unit)
                call build.official.bat --verbosity=diagnostic --target=CI --testExecutionType=%%TestType%% --shouldRunOpenCover=false
            """.trimIndent()
        }
    }

    triggers {
        vcs {
            branchFilter = ""
        }
        schedule {
            schedulingPolicy = daily {
                hour = 2
                minute = 50
            }
            branchFilter = """
                +:<default>
            """.trimIndent()
            triggerBuild = always()
			withPendingChangesOnly = false
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
