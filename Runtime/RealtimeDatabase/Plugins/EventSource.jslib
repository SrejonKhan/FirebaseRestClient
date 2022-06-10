var es = {
  $ES: {},
  esInstance: {},
  guid: "",
  EventSourceInit: function (guidPtr, urlPtr, callback) {
    this.guid = Pointer_stringify(guidPtr);
    var url = Pointer_stringify(urlPtr)
      .replace(/\+/g, "%2B")
      .replace(/%252[fF]/gi, "%2F");

    this.esInstance = new EventSource(url);

    // patch event
    this.esInstance.addEventListener("patch", function (e) {
      console.log(e);
      HandleEventCallback(e, "put", callback);
    });

    // put event
    this.esInstance.addEventListener("put", function (e) {
      console.log(e);
      HandleEventCallback(e, "put", callback);
    });

    function HandleEventCallback(e, name, cb) {
      var idBuffer = GetStrBuffer(this.guid);
      var nameBuffer = GetStrBuffer(name);
      var dataBuffer = GetStrBuffer(e.data);
      Module.dynCall_viii(cb, idBuffer, nameBuffer, dataBuffer);
    }

    function GetStrBuffer(str) {
      var bufferSize = lengthBytesUTF8(str) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(str, buffer, bufferSize);
      return buffer;
    }

    console.log("Listening to Event Source.");
  },
  EventSourceIsSupported: function () {
    return typeof EventSource !== "undefined";
  },
  EventSourceClose: function () {
    this.esInstance.close();
    console.log("Event Source closed - " + this.guid);
  },
};

autoAddDeps(es, "$ES");
mergeInto(LibraryManager.library, es);
