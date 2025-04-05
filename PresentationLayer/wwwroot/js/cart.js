document.addEventListener('DOMContentLoaded', function () {
    var plusBtns = document.querySelectorAll('.plus');
    var minusBtns = document.querySelectorAll('.minus');
    var quantityInputs = document.querySelectorAll('.quantity-input');

    function validateQuantity(quantity, stockQuantity) {
        if (quantity < 1) return 1;
        if (quantity > stockQuantity) return stockQuantity;
        return quantity;
    }

    function updateQuantityInput(input, newValue, stockQuantity) {
        const validatedQuantity = validateQuantity(newValue, stockQuantity);
        input.value = validatedQuantity;
        return validatedQuantity;
    }

    quantityInputs.forEach(function (input) {
        input.addEventListener('change', function () {
            const id = this.getAttribute('data-productId');
            const stockQuantity = parseInt(this.getAttribute('data-stockQuantity'));
            const newQuantity = updateQuantityInput(this, parseInt(this.value), stockQuantity);
            
            if (newQuantity !== parseInt(this.value)) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Invalid Quantity',
                    text: `Quantity must be between 1 and ${stockQuantity}`
                });
            }
            
            updateQuantity(`/cart-item/update/${id}?quantity=${newQuantity}`, id);
        });
    });

    async function updateQuantity(url, id) {
        try {
            var response = await fetch(url, {
                method: 'PATCH'
            });

            if (response.ok) {
                var data = await response.json();

                if (data.success) {
                    var quantityElement = document.querySelector('.quantity-input[data-productId="' + id + '"]');
                    quantityElement.value = data.quantity;

                    var subTotalElement = document.querySelector('#subTotal[data-productId="' + id + '"]');
                    subTotalElement.innerText = '$' + data.subTotal.toFixed(2);

                    var cartTotalElement = document.querySelector('#cart-total');
                    cartTotalElement.innerText = '$' + data.total.toFixed(2);
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: data.message || 'Failed to update quantity'
                    });
                }
            } else {
                throw new Error('Network response was not ok');
            }

        } catch (error) {
            console.error('Error:', error);
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'An error occurred while updating the cart'
            });
        }
    }

    plusBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var id = btn.getAttribute('data-productId');
            var stockQuantity = parseInt(btn.getAttribute('data-stockQuantity'));
            var quantityInput = document.querySelector('.quantity-input[data-productId="' + id + '"]');
            var currentQuantity = parseInt(quantityInput.value);
            
            if (currentQuantity < stockQuantity) {
                updateQuantity(`/cart-item/plus/${id}`, id);
            } else {
                Swal.fire({
                    icon: 'warning',
                    title: 'Stock Limit',
                    text: `Only ${stockQuantity} items available in stock`
                });
            }
        });
    });

    minusBtns.forEach(function (btn) {
        btn.addEventListener('click', function () {
            var id = btn.getAttribute('data-productId');
            var quantityInput = document.querySelector('.quantity-input[data-productId="' + id + '"]');
            var currentQuantity = parseInt(quantityInput.value);
            
            if (currentQuantity > 1) {
                updateQuantity(`/cart-item/minus/${id}`, id);
            } else {
                Swal.fire({
                    title: 'Remove Item?',
                    text: 'Do you want to remove this item from your cart?',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Yes, remove it!'
                }).then((result) => {
                    if (result.isConfirmed) {
                        window.location.href = `/ShoppingCart/Remove/${id}`;
                    }
                });
            }
        });
    });
});