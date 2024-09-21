// Hàm để thiết lập active cho menu
function setActiveMenu() {
	const activeMenu = localStorage.getItem('activeMenu');
	if (activeMenu) {
		allSideMenu.forEach(item => {
			const li = item.parentElement;
			if (item.getAttribute('href') === activeMenu) {
				li.classList.add('active');
			} else {
				li.classList.remove('active');
			}
		});
	}
}

const allSideMenu = document.querySelectorAll('#sidebar .side-menu.top li a');

allSideMenu.forEach(item => {
	const li = item.parentElement;

	// Thêm sự kiện click
	item.addEventListener('click', function () {
		allSideMenu.forEach(i => {
			i.parentElement.classList.remove('active');
		});
		li.classList.add('active');

		// Lưu trạng thái vào localStorage
		localStorage.setItem('activeMenu', item.getAttribute('href'));
	});
});

// Gọi hàm để thiết lập active khi trang tải
setActiveMenu();


// TOGGLE SIDEBAR
const menuBar = document.querySelector('#content nav .bx.bx-menu');
const sidebar = document.getElementById('sidebar');

menuBar.addEventListener('click', function () {
	sidebar.classList.toggle('hide');
})

if (window.innerWidth < 768) {
	sidebar.classList.add('hide');
}

const switchMode = document.getElementById('switch-mode');

switchMode.addEventListener('change', function () {
	if (this.checked) {
		document.body.classList.add('dark');
	} else {
		document.body.classList.remove('dark');
	}
})