$(document).ready(function () {
    loadData();
});

function loadData() {
    $('#productTable').DataTable({
        ajax: '/api/products',
        responsive: true,
        columns: [
            { data: 'id', width: "5%" },
            { data: 'name', width: "20%" },
            {
                data: 'description',
                width: "30%",
                className: 'd-none d-lg-table-cell'
            },
            {
                data: 'price',
                width: "15%",
                render: function (data) {
                    return '$' + parseFloat(data).toFixed(2);
                }
            },
            //{
            //    data: 'image',
            //    width: "20%",
            //    render: function (data) {
            //        return `<img src="/images/${data}" class="object-fit-contain card-img w-100" style="height:150px" />`;
            //    }
            //},
            {
                data: 'id',
                width: "20%",
                render: function (data) {
                    return `
                            <div class="d-flex justify-content-end gap-2">
                                <a href="/Product/Update/${data}" class="btn btn-primary btn-sm">
                                    <i class="bi bi-pencil"></i> Edit
                                </a>
                                <a onclick="deleteProduct('/api/delete/${data}')" class="btn btn-danger btn-sm">
                                    <i class="bi bi-trash"></i> Delete
                                </a>
                            </div>`;
                }
            }
        ],
        language: {
            emptyTable: "No products found"
        },
        order: [[0, 'desc']],
        pageLength: 10,
        lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "All"]]
    });
}

async function deleteProduct(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                const response = await fetch(url, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                const data = await response.json();

                if (response.ok) {
                    if (data.success) {
                        $('#productTable').DataTable().ajax.reload();
                        Swal.fire({
                            icon: 'success',
                            title: 'Success',
                            text: data.message
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: data.message
                        });
                    }
                } else {
                    throw new Error('Something went wrong');
                }
            } catch (error) {
                console.log(error);
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'An error occurred while deleting the product'
                });
            }
        }
    });
}




