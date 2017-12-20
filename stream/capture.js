
//var audio = document.createElement('audio');
var audio = document.getElementById("audio");
audio.play();


var audioCtx = new(window.AudioContext || window.webkitAudioContext)();
var analyser = audioCtx.createAnalyser();

// ...
var source = audioCtx.createMediaElementSource(audio);
source.connect(analyser);
analyser.connect(audioCtx.destination);
analyser.fftSize = 2048;
var bufferLength = analyser.frequencyBinCount;
var dataTimeArray = new Float32Array(bufferLength);
var dataFreqArray = new Float32Array(bufferLength);

 setInterval(function(){ 


	analyser.getFloatTimeDomainData(dataTimeArray);
	analyser.getFloatFrequencyData(dataFreqArray);


}, 200);