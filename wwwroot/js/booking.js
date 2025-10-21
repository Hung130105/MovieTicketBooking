document.addEventListener("DOMContentLoaded", function () {
    const moviePriceEl = document.getElementById("moviePrice");
    const pricePerSeat = parseFloat(moviePriceEl ? moviePriceEl.value : 0);

    const seatCheckboxes = document.querySelectorAll(".seat-checkbox");
    const ticketInput = document.getElementById("ticketCount");
    const neededSeatsEl = document.getElementById("neededSeats");
    const selectedSeatsEl = document.getElementById("selectedSeats");
    const selectedSeatsList = document.getElementById("selected-seats-list");
    const totalPriceEl = document.getElementById("totalPrice");
    const totalAmountEl = document.getElementById("total-amount");
    const seatCountEl = document.getElementById("seat-count");
    const seatError = document.getElementById("seat-error");
    const bookingForm = document.getElementById("bookingForm");
    const selectedSeatInputsContainer = document.getElementById("selectedSeatInputs");

    let selectedSeats = [];

    // --- Hàm cập nhật hiển thị ---
    function updateSummary() {
        const total = selectedSeats.length * pricePerSeat;

        if (selectedSeatsEl) selectedSeatsEl.textContent = selectedSeats.length;
        if (seatCountEl) seatCountEl.textContent = selectedSeats.length;

        if (totalPriceEl)
            totalPriceEl.textContent = "Tổng tiền: " + total.toLocaleString("vi-VN") + " VNĐ";
        if (totalAmountEl)
            totalAmountEl.textContent = total.toLocaleString("vi-VN") + " VNĐ";

        if (selectedSeatsList)
            selectedSeatsList.textContent =
                selectedSeats.length > 0
                    ? selectedSeats.map(s => s.row + s.number).join(", ")
                    : "Chưa chọn ghế";
    }

    // --- Khi thay đổi số lượng vé ---
    if (ticketInput) {
        ticketInput.addEventListener("input", function () {
            const count = parseInt(ticketInput.value) || 1;
            if (neededSeatsEl) neededSeatsEl.textContent = count;

            // Nếu chọn quá thì bỏ bớt ghế
            while (selectedSeats.length > count) {
                const last = selectedSeats.pop();
                const cb = document.getElementById("seat-" + last.id);
                if (cb) cb.checked = false;
            }
            updateSummary();
        });
    }

    // --- Khi chọn / bỏ chọn ghế ---
    seatCheckboxes.forEach(cb => {
        cb.addEventListener("change", function () {
            const limit = parseInt(ticketInput.value) || 1;
            const seatId = this.value;
            const row = this.dataset.row;
            const number = this.dataset.seatNumber; // đúng tên dataset

            if (this.checked) {
                if (selectedSeats.length >= limit) {
                    alert(`Bạn chỉ được chọn tối đa ${limit} ghế!`);
                    this.checked = false;
                    return;
                }
                selectedSeats.push({ id: seatId, row, number });
            } else {
                selectedSeats = selectedSeats.filter(s => s.id !== seatId);
            }
            updateSummary();
        });
    });

    // --- Trước khi submit form ---
    if (bookingForm) {
        bookingForm.addEventListener("submit", function (e) {
            const requiredCount = parseInt(ticketInput.value) || 1;

            if (selectedSeats.length !== requiredCount) {
                e.preventDefault();
                if (seatError)
                    seatError.textContent = `Vui lòng chọn đúng ${requiredCount} ghế.`;
                window.scrollTo({ top: 0, behavior: "smooth" });
                return;
            }

            if (selectedSeatInputsContainer) {
                selectedSeatInputsContainer.innerHTML = "";
                selectedSeats.forEach(s => {
                    const inp = document.createElement("input");
                    inp.type = "hidden";
                    inp.name = "SelectedSeatIds";
                    inp.value = s.id;
                    selectedSeatInputsContainer.appendChild(inp);
                });
            }
        });
    }

    updateSummary();
});