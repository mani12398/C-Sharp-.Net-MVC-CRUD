function myFunction() {
    var input, filter, table, tbody, tr, td, i, j, txtValue;
    input = document.getElementById("searchInput");
    filter = input.value.toUpperCase();
    table = document.getElementById("products"); // Replace "products" with your actual table ID
    tbody = table.getElementsByTagName("tbody")[0]; // Select tbody element

    // Show all tbody rows
    tr = tbody.getElementsByTagName("tr");

    for (i = 0; i < tr.length; i++) {
        var rowVisible = false;
        for (j = 0; j < tr[i].cells.length; j++) {
            td = tr[i].cells[j];
            if (td) {
                txtValue = td.textContent || td.innerText;
                if (txtValue.toUpperCase().indexOf(filter) > -1) {
                    rowVisible = true;
                    break; // Break out of the inner loop since a match is found
                }
            }
        }
        // Toggle row display based on visibility
        if (rowVisible) {
            tr[i].style.display = "";
        } else {
            tr[i].style.display = "none";
        }
    }
}
