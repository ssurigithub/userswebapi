# scenarios:

1. A trigger => tells a pipeline to run
2. pipeline is made up of one or more stages.
3. stage is a way of organizing your jobs.
4. each job runs in 1 agent. a job can be agentless.
5. each agent runs a job that contains one or more steps.
6. a step can be task or a script - is the smallest building block of a pipeline.
7. task is a prepackaged script that performs an action.
8. build artifact is a collection of files or packages published by a run.

trigger => triggers pipeline => has one or more stages => has one or more jobs => runs on agents => contains one ore more steps

# YAML- https://learnxinyminutes.com/docs/yaml/

1.  key:value
2.  key with spaces: value
3.  sequence: // like a list or array
    - Item 1
    - Item 2
    - .5
    - key: value
    - // new sequence
      - additional sequence

pool:
vmImage: 'ubuntu-16.04'

1. stages:
   - stage: A
     jobs:
     - job: A1
       steps:
       - [script | bash | pwsh | powershell | task | chekout | templateReference]
     - job: A2
   - stage: B

- stage

# use script to execute any commands

- echo
- git version
- dotnet version
- predefined build variables.

# triggers

    - branch
    - include / exclude
    - cron scheduler
