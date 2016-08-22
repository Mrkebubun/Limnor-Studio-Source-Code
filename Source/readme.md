This folder contains source code for Limnor Studio.

Development Solution:
LimnorStudioSolution/LimnorStudioDev.sln

Distribution Solution:
LimnorStudioSolution/LimnorStudioDist.sln

Limnor Studio distribution relies on WIX. Please download and install WIX from http://www.wix.com/
Once installed WIX, modify postCompile/setupProj.xml to specify WIX installation folder via "wixbin" attribute of "root" element, i.e.
<root wixbin="C:\Program Files (x86)\WiX Toolset v3.6\bin">





