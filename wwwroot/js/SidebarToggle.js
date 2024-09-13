$(document).ready(function () {
    // Remove the no-transition class after the page has fully loaded
    document.documentElement.classList.remove("sidebar-collapsed-initial");

    // Check the localStorage for sidebar state
    if (localStorage.getItem("sidebarState") === "collapsed") {
        $("#sidebar").addClass("sidebar-collapsed");
        $(".container").css("margin-left", "60px"); // Adjust container margin for collapsed state
    } else {
        $("#sidebar").removeClass("sidebar-collapsed");
        $(".container").css("margin-left", "250px"); // Adjust container margin for expanded state
    }

    // Toggle the sidebar and save the state in localStorage
    $("#sidebarToggle").on("click", function () {
        $("#sidebar").toggleClass("sidebar-collapsed");

        if ($("#sidebar").hasClass("sidebar-collapsed")) {
            localStorage.setItem("sidebarState", "collapsed");
            $(".container").css("margin-left", "60px"); // Match the collapsed width
        } else {
            localStorage.setItem("sidebarState", "expanded");
            $(".container").css("margin-left", "250px"); // Match the expanded width
        }
    });
});
