function var_roadname() {
	var roadname = "";
	var CanStart = false;
	roadname = document.getElementById('roadcode').value;
	console.log(roadname);
	if (roadname != "" && roadname != null) {
		if (document.getElementById('CNMan1').checked == true) {
			Formium.external.Roadinfo.Roadname(roadname + " -r");
			CanStart = true;
		} else {
			Formium.external.Roadinfo.Roadname(roadname);
			CanStart = true;
		}
		if (document.getElementById('CNMan2').checked == true) {
			Formium.external.Roadinfo.Roadname(roadname + " _express");
			CanStart = true;
		} else {
			Formium.external.Roadinfo.Roadname(roadname);
			CanStart = true;
		}
		if (document.getElementById('CNMan3').checked == true) {
			Formium.external.Roadinfo.Roadname(roadname + " _compact");
			CanStart = true;
		} else {
			Formium.external.Roadinfo.Roadname(roadname);
			CanStart = true;
		}
		if (document.getElementById('CNMan3').checked == true && document.getElementById('CNMan2').checked == true) {
			Formium.external.Roadinfo.Roadname(roadname + " _compact _express");
			CanStart = true;
		} else {
			Formium.external.Roadinfo.Roadname(roadname);
			CanStart = true;
		}
	}
	//Formium.external.Roadinfo.roadname = document.getElementById('roadname').value;
	if (document.getElementById('roadcode').value != "") {
		if (CanStart == true) {
			Lobibox.notify('info', {
				pauseDelayOnHover: true,
				continueDelayOnInactiveTab: false,
				position: 'top right',
				icon: 'bx bx-info-circle',
				msg: 'Generating'
			});
			Formium.external.Generat.Generat();
			loader_1();
		} else {
			Lobibox.notify('warning', {
				pauseDelayOnHover: true,
				continueDelayOnInactiveTab: false,
				position: 'top right',
				icon: 'bx bx-info-circle',
				msg: 'Please enter your modules name'
			});
		}
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

function loader_1() {
	document.getElementById('Gen_Start_1').className = 'spinner-border spinner-border-sm';
}

function testJS() {
	console.log(Formium.external.Testget.test("123441414141414"))
}

function go() {
	window.history.go(-1);
}

function setIIMtable() {
	var tbody = document.getElementById('IIM');
	var mname = Formium.external.IIM.modulesname;
	var tname = Formium.external.IIM.modulestype;
	tbody.innerHTML += '<tr><td>' + mname + '</td><td>' + tname + '</td><td style="color:#16E15E">Installed</td></tr>';
}

function TakeGenerat(){
	if (document.getElementById('oneway').checked == true){
		Formium.external.Select_Gen.Generat("one")
	}else if(document.getElementById('twoway').checked == true){
		Formium.external.Select_Gen.Generat("two")
	}else if(document.getElementById('Gall') == true){
		Formium.external.Select_Gen.Generat("Gall")
	}else{
		Lobibox.notify('warning', {
			pauseDelayOnHover: true,
			continueDelayOnInactiveTab: false,
			position: 'top right',
			icon: 'bx bx-info-circle',
			msg: 'Please select generat mode'
		});
	}
}
function AddtoReadyList(){
	var ttbody = document.getElementById('RL_IIM');
	var aname = Formium.external.Select_Add.addname;
	var atype = Formium.external.Select_Add.atype;
	var amode = Formium.external.Select_Add.amode;
	ttbody.innerHTML += '<tr><td>' + aname + '</td><td>' + atype + '</td><td>'+ amode +'</td></tr>';
}
function SavesReady(){
	var lists = new Array();
	var objtalbe = document.getElementById('RL_BIIM');
	for(var i = 0; i<objtalbe.rows.length; i++){
		lists[i] = objtalbe.rows[i].cells[0].innerHTML;
		Formium.external.Select_Save.savesname += lists[i]+",";
		console.log(lists[i]);
		console.log(Formium.external.Select_Save.savesname);
	}
	Formium.external.Select_Save.SaveList(Formium.external.Select_Save.savesname);
	Lobibox.notify('success', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-check-circle',
		msg: 'Saved'
	});
}
function importsaves(){
	Formium.external.Select_Import.ImportSave();
	Lobibox.notify('success', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top center',
		icon: 'bx bx-check-circle',
		msg: 'Imported complete'
	});
	// document.getElementById('CNMan2').Disabled = true;
	// document.getElementById('CNMan3').Disabled = true;
}