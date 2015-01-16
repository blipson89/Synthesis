SYNTHESIS README

Thanks for installing Synthesis! Here are some tips to get you started:

First, you should set up your configuration. 
Open App_Config/Include/Synthesis.config and tweak your namespaces and output locations to your liking.

Next, you'll want to run a build so the Synthesis assemblies are in your /bin folder and don't cause an error.

To generate your model, go to /synthesis.aspx on your project in a web browser.
Should this URL not work, you may need to change the activationUrl at the bottom of the config file to not include the aspx and try /synthesis

Finally, in Visual Studio set the project to Show All Files and include your generated files for compilation.

NOTE: your project must reference Sitecore.Kernel, Sitecore.ContentSearch, and Sitecore.ContentSearch.Linq for the model to compile.

Want deeper documentation? The Synthesis Wiki on GitHub has you covered: https://github.com/kamsar/Synthesis/wiki

Have questions? Tweet @kamsar.

Found a bug? Send me a pull request on GitHub if you're feeling awesome: https://github.com/kamsar/Synthesis
(or an issue if you're feeling lazy)