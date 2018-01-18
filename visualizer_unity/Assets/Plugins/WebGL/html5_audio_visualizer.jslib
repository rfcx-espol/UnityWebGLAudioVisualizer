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
	SelectStation: function(station){
		set_station(station);
	},
	StopStation: function(){
		stop_station();
	}
}

mergeInto(LibraryManager.library, csharp);