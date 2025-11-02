document.addEventListener("DOMContentLoaded", () => {
    // 🎯 التعامل مع زر Add to Cart
    document.querySelectorAll(".add-to-cart-form").forEach(form => {
        form.addEventListener("submit", async e => {
            e.preventDefault();
            const productId = form.dataset.productId;

            const response = await fetch(`/Cart/AddToCart?productId=${productId}`, { method: "POST" });
            if (response.ok) {
                await updateCartOffcanvas();
                showToast("✅ Product added to cart successfully!");
            } else {
                showToast("❌ Failed to add product!", true);
            }
        });
    });
});

// 🔄 تحديث محتوى الكارت
async function updateCartOffcanvas() {
    const container = document.getElementById("cartContainer");
    if (!container) return;

    const response = await fetch("/Cart/GetCartPartial");
    if (response.ok) {
        const html = await response.text();
        container.innerHTML = html;

        // 🔢 تحديث عداد السلة
        const tempDiv = document.createElement("div");
        tempDiv.innerHTML = html;
        const totalItems = tempDiv.querySelectorAll("li.list-group-item").length;

        const badge = document.querySelector(".btn .badge");
        if (badge) {
            if (totalItems > 0) {
                badge.textContent = totalItems;
                badge.classList.remove("d-none");
            } else {
                badge.classList.add("d-none");
            }
        }
    }
}

// ✅ إظهار Toast
function showToast(message, isError = false) {
    const toastEl = document.getElementById("liveToast");
    const toastMsg = document.getElementById("toastMessage");

    toastMsg.textContent = message;
    toastEl.classList.remove("text-bg-success", "text-bg-danger");
    toastEl.classList.add(isError ? "text-bg-danger" : "text-bg-success");

    const toast = new bootstrap.Toast(toastEl);
    toast.show();
}
