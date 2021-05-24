var roadcode;
function var_roadname() {
	roadcode = document.getElementById('roadcode').value;
	if (document.getElementById('CNMan1').checked == true){
		roadcode += " -r";
	}
	if (document.getElementById('CNMan2').checked == true){
		roadcode += " _express";
	}
	if (document.getElementById('CNMan3').checked == true){
		roadcode += " _compact";
	}
	//Formium.external.Roadinfo.Roadcode = document.getElementById('roadcode').value;
	if (roadcode != null) {
		Formium.external.Roadinfo.Roadcode = roadcode;
		Lobibox.notify('info', {
			pauseDelayOnHover: true,
			continueDelayOnInactiveTab: false,
			position: 'top right',
			icon: 'bx bx-info-circle',
			msg: 'Generating'
		});
	} else {
		Lobibox.notify('warning', {
			pauseDelayOnHover: true,
			continueDelayOnInactiveTab: false,
			position: 'top right',
			icon: 'bx bx-info-circle',
			msg: 'Please enter your modules name'
		});
	}
}
