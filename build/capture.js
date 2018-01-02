
var time_pedro;
var frequency_pedro;
var radio_station = 0;
var server_address = "http://192.168.100.100:8000/station";
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
time_pedro = new Float32Array(bufferLength);
frequency_pedro = new Float32Array(bufferLength);

 setInterval(function(){ 


	analyser.getFloatTimeDomainData(time_pedro);
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