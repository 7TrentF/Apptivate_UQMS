class ConditionalDisplay {
    constructor() {
        this.role = document.querySelector("input[name='Role']").value;
        this.courseField = document.getElementById("courseField");
        this.positionField = document.getElementById("positionField");

        this.init();
    }

    init() {
        // Always show the Course field
        this.showCourseField();

        // Show or hide the Position field based on the role
        if (this.role === "Student") {
            this.hidePositionField();
        } else if (this.role === "Staff") {
            this.showPositionField();
        }
    }

    showCourseField() {
        this.courseField.style.display = "block";
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
