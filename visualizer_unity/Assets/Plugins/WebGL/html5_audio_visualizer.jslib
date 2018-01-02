var csharp = {
	Initialize: function() {
		// window.alert(pedro);
	},
	PrintFloatArray: function(array, size) {
		for(var i=0;i<size;i++){
			HEAPF32[(array>>2)+i] = sample_pedro[i];
		}
	},
	PrintFloatArrayFreq: function(array, size) {
		for(var i=0;i<size;i++){
			HEAPF32[(array>>2)+i] = frequency_pedro[i];
		}
	},
	SendAudioPath: function (str) {
		window.alert(Pointer_stringify(str));
	}
}

mergeInto(LibraryManager.library, csharp);