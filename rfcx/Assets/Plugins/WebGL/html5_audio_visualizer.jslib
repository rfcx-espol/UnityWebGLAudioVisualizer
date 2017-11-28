var csharp = {
	Initialize: function() {
		window.alert(pedro);
	},
	PrintFloatArray: function(array, size) {
		window.alert(pedro);
		for(var i=0;i<size;i++){
			HEAPF32[(array>>2)+i] = pedro[i]; //Confused about what this does or how it works
		}
	}
}

mergeInto(LibraryManager.library, csharp);
