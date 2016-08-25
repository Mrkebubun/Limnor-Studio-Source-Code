# **Limnor-Studio**: Visual-Codeless-Programming-System
It is a generic-purpose visual codeless programming system. It can be used to develop various Windows standalone applications and web applications.

Home page: http://www.limnor.com

User forum: http://www.limnor.com/LimnorForum.html

Documents: https://github.com/Limnor/Limnor-Studio-Documents

Contact: support@limnor.com

Contributors:
- Bob Limnor
- Jian Wang
- Flora Lee
- Paul Chen

New contributors are welcome!

Download and use the source code:

1. Click "Clone or download" button
2. Click "Download ZIP" and wait for a zip file being downloaded to your computer
3. Unzip the downloaded zip file to a folder, keeping the folder structure
4. Use Microsoft Visual Studio 2012 to open solutions in folder LimnorStudioSolutions
  - LimnorStudioSolutions/LimnorStudioDev.sln includes most of projects for development and debugging, but it does not include an installation project for distributing Limnor Studio.
  - Start Microsoft Visual Studio 2012 as an **Administrator**. Admin privilege is required for debugging.
  - On loading LimnorStudioDev.sln, set project "**LimnorLite32**" as the **StartUp project**. Build the solution and you may start debugging Limnor Studio under Microsoft Visual Studio 2012.
  - LimnorStudioSolutions/LimnorStudioDist.sln includes a project for generating MSI for Limnor Studio distribution.
  - Be sure to set build mode to **Release** on loading LimnorStudioDist.sln. This solution does not work in Debug mode. Build the solution and you will get Limnor Studio installer file LimnorStudio5Net4.MSI in folder postCompile\Release.
