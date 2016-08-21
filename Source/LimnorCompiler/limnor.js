String.prototype.startsWith = function(str) { return (this.match("^" + str) == str) }

        //serverPage must be set including html
	function callServer(methodId, data) {
            var self = this;
            if (window.XMLHttpRequest) {
                self.xmlHttpReq = new XMLHttpRequest();
            }
            else if (window.ActiveXObject) {
                self.xmlHttpReq = new ActiveXObject("Microsoft.XMLHTTP");
            }
            
            self.xmlHttpReq.open('POST', serverPage, true);
            self.xmlHttpReq.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
            self.xmlHttpReq.onreadystatechange = function() {
                if (self.xmlHttpReq.readyState == 4) {
                    processServerResponse(self.xmlHttpReq.responseText);
                }
            }
            if (data == undefined) {
                methodId = 'methodid=' + methodId
            }
            else if (data.length == 0) {
                methodId = 'methodid=' + methodId
            }
            else {
                methodId = 'methodid=' + methodId + '&' + data;
            }
            self.xmlHttpReq.send(methodId);
        }
        //execute client scripts passed from server
        function processServerResponse(str) {
            if (str.startsWith("<html>")) {
                document.write(str);
            }
            else {
                //divResponse is a div
                var idResponse = 'idResponse';
                var divResponse = document.getElementById(idResponse);
                if (divResponse == undefined) {
                    divResponse = document.createElement('div');
                    divResponse.setAttribute('id', idResponse);
                    divResponse.setAttribute('style', 'visibility:hidden');
                    document.body.appendChild(divResponse);
                }
                if (str.length > 0) {
                    //set html including scripts to the div
                    divResponse.innerHTML = str;
                    //get all scripts
                    var d = divResponse.getElementsByTagName('script');
                    var t = d.length;
                    for (var x = 0; x < t; x++) {
                        //execute the script
                        eval(d[x].text);
                    }
                    divResponse.innerHTML = '';
                }
            }
        }
