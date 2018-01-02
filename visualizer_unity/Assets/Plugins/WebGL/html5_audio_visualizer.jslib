var csharp = {
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
<<<<<<< HEAD
	SendAudioPath: function (str) {
		window.alert(Pointer_stringify(str));
=======
	SelectStation: function(station){
		set_station(station);
>>>>>>> 9b056b7fc463c7cca71c7f651b8b129b3a06826e
	}
}

mergeInto(LibraryManager.library, csharp);