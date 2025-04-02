
$(document).ready(function () {
    loadData();
});

function loadData() {
    $('#productTable').DataTable({
        ajax: '/api/products',
        columns: [
            { data: 'id' , width: "5%"},
            { data: 'name' , width: "10%"},
            { data: 'description', width: "30%" },
            { data: 'price' , width : "15%"},
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
                            <div class="text-center d-flex align-items-center justify-content-center gap-2">
                                <a href="/Product/Update/${data}" class="btn btn-success text-white w-50" style="cursor:pointer;">
                                    Edit
                                </a>
                                <a onclick="deleteProduct('/api/delete/${data}')" class="btn btn-danger text-white w-50" style="cursor:pointer;">
                                    Delete
                                </a>
                            </div>`;

                }
            }
        ]
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
                        toastr.success(`${data.message}`, 'Success');
                    } else {
                        toastr.error(`${data.message}`, 'Error!');
                    }

                } else {
                    throw new Error('Something went wrong');
                }
            } catch (error) {
                console.log(error);
            }
        }
    });
}




