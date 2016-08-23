This folder contains source code for Limnor Studio.

Development Solution:
LimnorStudioSolutions/LimnorStudioDev.sln

Distribution Solution:
LimnorStudioSolutions/LimnorStudioDist.sln

Limnor Studio distribution relies on WIX. Please download and install WIX from http://wixtoolset.org/releases/
Once installed WIX, modify postCompile/setupProj.xml to specify WIX installation folder via "wixbin" attribute of "root" element, i.e.
```
<root wixbin="C:\Program Files (x86)\WiX Toolset v3.6\bin">
```




