function Check_noti_group() {
	Lobibox.notify('success', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-check-circle',
		msg: 'Check Complete!'
	});
	Lobibox.notify('info', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-info-circle',
		msg: 'Now you can Generat your onw modules road.'
	});
}

function Generator_success() {
	Lobibox.notify('success', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-x-circle',
		msg: 'Generat completed! Now you can open CSL to import your modules.'
	});
}

function Generator_error() {
	Lobibox.notify('error', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-x-circle',
		msg: 'Generat failed.'
	});
}


function Before_error() {
	Lobibox.notify('error', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-x-circle',
		msg: 'Environment Check Fail! Please check your environment then try again later.'
	});
}

function Path_error() {
	Lobibox.notify('warning', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-x-circle',
		msg: 'Path is not definde'
	});
}

function RIMods_error() {
	Lobibox.notify('warning', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-x-circle',
		msg: 'Roadimporter is not definde'
	});
}

function CRM_error() {
	Lobibox.notify('warning', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-x-circle',
		msg: 'CSUR Master is not definde'
	});
}

function Blender_error() {
	Lobibox.notify('warning', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-x-circle',
		msg: 'Blender is not definde'
	});
}

function TPINS_error() {
	Lobibox.notify('warning', {
		pauseDelayOnHover: true,
		continueDelayOnInactiveTab: false,
		position: 'top right',
		icon: 'bx bx-x-circle',
		msg: 'Textures Pack is not definde'
	});
}