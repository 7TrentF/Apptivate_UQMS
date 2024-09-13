<div class="form-group">
    <label for="fileUpload">Upload Supporting Documents</label>
    <input type="file" id="fileUpload" name="files" multiple>
</div>

<script>
    document.getElementById('fileUpload').addEventListener('change', function () {
        var fileList = this.files;
        for (var i = 0; i < fileList.length; i++) {
            console.log("Selected file: " + fileList[i].name);
        }
    });
</script>
