# Introduction #

A short guide on how to checkout and build duplicati from source.

# Details #

duplicati is a C# project for Windows, with code access via Git. All software can be obtained for free (though restricted via their own licenses). <br>

Anonymous users have the ability to checkout a read-only copy of the code and can make changes locally. For commit access, please contact duplicati’s Project Owner.<br>

<h2>Tools</h2>

The tools required to checkout and compile duplicati are:<br>
<ul><li>A Git Client, Windows and Mac users can try <a href='http://www.sourcetreeapp.com/'>Source Tree</a>, linux users have other options for Git<br>
</li><li>C# .NET Build environement</li></ul>

<h2>Getting the source via Git</h2>

See <a href='https://code.google.com/p/duplicati/source/checkout'>the checkout page for a Git clone command</a>.<br>
<br>
<h2>C# .NET</h2>
Windows users can try either <a href='http://www.microsoft.com/express/Downloads/#2010-Visual-CS'>VS Express</a>, <a href='https://xamarin.com/studio'>Xamarin Studio</a> or <a href='http://www.icsharpcode.net/OpenSource/SD/Default.aspx'>SharpDevelop</a>.<br>
Mac Users will want to use <a href='https://xamarin.com/studio'>Xamarin Studio</a>.<br>
Linux users will want to use <a href='http://monodevelop.com/'>MonoDevelop</a>, unless you just want to build, in which case you should look for the xbuild utility in your package manager.<br>

Duplicati should build straight away (all batteries included) on all three platforms, using either xbuild, msbuild or an IDE. The MacTrayIcon project only builds with Xamarin Studio on OSX, but it will be ignored on other platforms and by xbuild/msbuild.<br>
<br>
If you use an IDE you should set the Duplicati.GUI.TrayIcon project as the startup project and just press run, and you should be ready (remember to exit running instances first).<br>
<br>
<h1>Building the installer and redistributable binaries</h1>

The "Installer" folder has some tools and various help files for building native packages for different operating systems. Generally the build is fully cross platform, so you can build on one platform and run on another. The tools for building the packages are usually only found on the OS in question though (e.g. you cannot make an RPM package on Windows).<br>
<br>
The OSX version needs Xamarin Studio to build the tray icon, but the rest works fine. The GTK version is also working on OSX, but does not look native.<br>
<br>
When building on Windows, you need to enable the preprocessor macro <code>__WindowsGTK__ </code> and link in the GTK libraries if you want a cross platform build with a GTK tray icon.