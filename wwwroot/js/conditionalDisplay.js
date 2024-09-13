class ConditionalDisplay {
    constructor() {
        // Get the role and the position field DOM elements
        this.role = document.getElementById("role").value;
        this.positionField = document.getElementById("positionField");

        this.init();
    }

    init() {
        // Show or hide the Position field based on the role
        if (this.role === "Student") {
            this.hidePositionField();
        } else {
            this.showPositionField();
        }
    }

    showPositionField() {
        this.positionField.style.display = "block";
    }

    hidePositionField() {
        this.positionField.style.display = "none";
    }
}

// Automatically initialize when the DOM is fully loaded
document.addEventListener("DOMContentLoaded", function () {
    new ConditionalDisplay();
});
