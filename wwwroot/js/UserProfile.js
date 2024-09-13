document.getElementById("toggleDetails").addEventListener("click", function () {
    const details = document.querySelectorAll(".profile-section");
    details.forEach(detail => detail.classList.toggle("hidden"));
});

// Add CSS to hide elements when toggled
const style = document.createElement('style');
style.innerHTML = `
  .hidden {
    display: none;
  }
`;
document.head.appendChild(style);
