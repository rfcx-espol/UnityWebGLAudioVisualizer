
var sample_pedro;
var frequency_pedro;
var radio_station = 0;
var server_address = "http://200.126.14.250:8000/station";
var extention = ".ogg";

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
sample_pedro = new Float32Array(bufferLength);
frequency_pedro = new Float32Array(bufferLength);

 setInterval(function(){ 


	analyser.getFloatTimeDomainData(sample_pedro);
	analyser.getFloatFrequencyData(frequency_pedro);


}, 200);


function set_station(station) {
	radio_station = station;
	new_address = server_address + radio_station.toString() + extention;
	audio.setAttribute("crossorigin", "anonymous");
	audio.setAttribute("src", new_address);
	audio.setAttribute("crossorigin", "anonymous");
	audio.play();
}

function stop_station(){
	audio.pause();
}