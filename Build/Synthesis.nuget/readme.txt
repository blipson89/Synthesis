SYNTHESIS README

Thanks for installing Synthesis! Here are some tips to get you started:

First, you should set up your base configuration.
Make a copy of App_Config\Include\Synthesis.LocalConfig.config.example, and rename it to .config.
Tweak your namespaces and output locations to your liking within your patch file.

Next, you'll want to run a build so the Synthesis assemblies are in your /bin folder and don't cause an error.

To generate your model, go to /synthesis.aspx on your project in a web browser.
Should this URL not work, you may need to change the activationUrl at the bottom of the config file to not include the aspx and try /synthesis
If you do not see an option to regenerate your model, make sure you have <compilation debug="true"> in your web.config as regeneration is disabled in release mode.

Finally, in Visual Studio set the project to Show All Files and include your generated code file for compilation.
This file will be located at the ModelOutputPath in your config patch you created.

NOTE: your project including the model classes must reference Sitecore.Kernel and Sitecore.ContentSearch.Linq for the model to compile.

Using modular architecture (e.g. Habitat) and need a model per module? See the example in the GitHub [README.md](https://github.com/blipson89/Synthesis) for directions to create a configuration registration.

Want deeper documentation? The Synthesis Wiki on GitHub has you covered: https://github.com/blipson89/Synthesis/wiki

Have questions? Tweet @bllipson or find me on Sitecore Community Slack.

Found a bug? Send me a pull request on GitHub if you're feeling awesome: https://github.com/blipson89/Synthesis
(or an issue if you're feeling lazy)