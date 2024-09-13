class Dropdown {
    constructor(departmentSelector, courseSelector, urlAction) {
        this.departmentSelector = $(departmentSelector);
        this.courseSelector = $(courseSelector);
        this.urlAction = urlAction;

        this.init();
    }

    init() {
        // Bind the change event to the department dropdown
        this.departmentSelector.change(() => {
            this.onDepartmentChange();
        });
    }

    onDepartmentChange() {
        // Get the selected department ID
        const departmentId = this.departmentSelector.val();

        // Make the AJAX call to get courses for the selected department
        $.ajax({
            url: this.urlAction, // URL for fetching courses by department
            type: 'GET',
            data: { departmentId: departmentId },
            success: (courses) => {
                this.updateCourseDropdown(courses);
            },
            error: (xhr, status, error) => {
                console.error("Error fetching courses: ", error);
            }
        });
    }

    updateCourseDropdown(courses) {
        // Clear the existing course dropdown
        this.courseSelector.empty();

        // Add the default "Select Course" option
        this.courseSelector.append('<option value="">-- Select Course --</option>');

        // Populate the course dropdown with new options
        $.each(courses, (index, course) => {
            this.courseSelector.append(`<option value="${course.courseID}">${course.courseName}</option>`);
        });
    }
}

// Usage
$(document).ready(function () {
    const departmentDropdown = '#department';
    const courseDropdown = '#course';
    const urlAction = '@Url.Action("GetCoursesByDepartment", "Registration")';

    const dropdown = new Dropdown(departmentDropdown, courseDropdown, urlAction);
});
