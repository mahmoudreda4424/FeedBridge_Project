const sidebar = document.querySelector(".sidebar ul");
const pageContent = document.querySelector(".content-page");

// Handling the page content
const handlePageContent = (li) => {
  const allLI = sidebar.querySelectorAll("li");
  allLI.forEach((el) => el.classList.remove("active"));
  li.classList.add("active");

  pageContent
    .querySelectorAll("div")
    .forEach((el) => el.classList.remove("active"));

  const targetDiv = pageContent.querySelector(
    `.${li.querySelector("p").textContent.trim().toLowerCase()}-page`
  );
  if (targetDiv) targetDiv.classList.add("active");
};

sidebar.querySelectorAll("li").forEach((li) => {
  li.addEventListener("click", (e) => handlePageContent(e.currentTarget));
});


// Notifications
const notificationsContainer = document.querySelector(".notifications-container");
const notificationsBox = document.querySelector(".notifications-box");

if (notificationsContainer && notificationsBox) {
  notificationsContainer.addEventListener("mouseover", () => {
    notificationsBox.style.display = "block";
  });

  notificationsContainer.addEventListener("mouseleave", () => {
    notificationsBox.style.display = "none";
  });
}
window.addEventListener("DOMContentLoaded", () => {
    const activeLi = sidebar.querySelector("li.active");
    if (activeLi) {
        handlePageContent(activeLi);
    }
});

document.querySelectorAll(".notif-link").forEach(link => {
    link.addEventListener("click", function (e) {
        e.preventDefault();
        const notifId = this.dataset.id;
        const role = document.body.dataset.role; // خلي في الـ body data-role="Admin" أو "Delivery"
        fetch(`/${role}/MarkNotificationAsRead?id=${notifId}`, { method: "POST" })
            .then(() => {
                // نقل المستخدم للرابط بعد التعليم كمقروء
                window.location.href = this.href;
            });
    });
});
