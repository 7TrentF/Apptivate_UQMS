document.addEventListener('DOMContentLoaded', function () {
    // Initialize smooth scroll
    document.documentElement.style.scrollBehavior = 'smooth';

    // Add loading animation
    function simulateLoading() {
        const sections = document.querySelectorAll('.profile-section');
        sections.forEach(section => {
            section.classList.add('loading');
            setTimeout(() => {
                section.classList.remove('loading');
            }, 1000);
        });
    }
    simulateLoading();

    // Animate profile sections on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    document.querySelectorAll('.profile-section').forEach(section => {
        section.style.opacity = '0';
        section.style.transform = 'translateY(30px)';
        section.style.transition = 'all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275)';
        observer.observe(section);
    });

    // Add year progress bar for students
    if (document.querySelector('.profile-section')) {
        const yearElements = document.querySelectorAll('.profile-section');
        yearElements.forEach(element => {
            const yearText = element.querySelector('p');
            if (yearText && yearText.textContent.includes('Year')) {
                const year = parseInt(yearText.textContent);
                if (!isNaN(year)) {
                    const progressBar = document.createElement('div');
                    progressBar.className = 'year-progress';
                    const progressInner = document.createElement('div');
                    progressInner.className = 'year-progress-bar';
                    progressInner.style.width = '0%';
                    progressBar.appendChild(progressInner);
                    element.appendChild(progressBar);

                    // Animate progress bar
                    setTimeout(() => {
                        progressInner.style.width = `${(year / 4) * 100}%`;
                    }, 500);
                }
            }
        });
    }

    // Add interactive tooltips
    const profileSections = document.querySelectorAll('.profile-section');
    profileSections.forEach(section => {
        section.addEventListener('click', function () {
            this.classList.toggle('expanded');
            const content = this.querySelector('p');
            if (content) {
                content.style.maxHeight = content.style.maxHeight ? null : content.scrollHeight + 'px';
            }
        });

        // Add hover effect
        section.addEventListener('mousemove', function (e) {
            const rect = this.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;

            this.style.transform = `
                perspective(1000px)
                rotateX(${(y - rect.height / 2) / 20}deg)
                rotateY(${-(x - rect.width / 2) / 20}deg)
                translateZ(10px)
            `;
        });

        section.addEventListener('mouseleave', function () {
            this.style.transform = 'none';
        });
    });

    // Add particle effect to the background
    function createParticle() {
        const particle = document.createElement('div');
        particle.className = 'particle';
        particle.style.cssText = `
            position: absolute;
            width: 5px;
            height: 5px;
            background: rgba(255, 255, 255, 0.5);
            border-radius: 50%;
            pointer-events: none;
        `;

        const startX = Math.random() * window.innerWidth;
        particle.style.left = startX + 'px';
        particle.style.top = '-5px';

        document.querySelector('.background').appendChild(particle);

        const animation = particle.animate([
            { transform: 'translateY(0) rotate(0deg)', opacity: 1 },
            { transform: `translateY(${window.innerHeight}px) rotate(360deg)`, opacity: 0 }
        ], {
            duration: 5000 + Math.random() * 5000,
            easing: 'linear'
        });

        animation.onfinish = () => particle.remove();
    }

    // Create particles periodically
    setInterval(createParticle, 500);

    // Add dynamic badges for roles and departments
    function addBadges() {
        const roleElement = document.querySelector('.role');
        if (roleElement) {
            const role = roleElement.textContent.trim();
            roleElement.innerHTML = `
                <span class="badge">${role}</span>
            `;
        }

        const departmentElements = document.querySelectorAll('.profile-section p');
        departmentElements.forEach(element => {
            if (element.textContent.includes('Department')) {
                const department = element.textContent.split(':')[1]?.trim();
                if (department) {
                    element.innerHTML = `Department: <span class="badge">${department}</span>`;
                }
            }
        });
    }
    addBadges();

    // Add typing effect to the header
    function typeEffect(element, text, speed = 100) {
        let i = 0;
        element.innerHTML = '';
        const typing = setInterval(() => {
            if (i < text.length) {
                element.innerHTML += text.charAt(i);
                i++;
            } else {
                clearInterval(typing);
            }
        }, speed);
    }

    const headerTitle = document.querySelector('.header h1');
    if (headerTitle) {
        const originalText = headerTitle.textContent;
        typeEffect(headerTitle, originalText);
    }

    // Add profile image hover effects
    const profileImage = document.querySelector('.profile-pic');
    if (profileImage) {
        profileImage.addEventListener('mouseover', function () {
            this.style.transform = 'scale(1.1) rotate(5deg)';
            this.style.boxShadow = '0 15px 35px rgba(0,0,0,0.3)';
        });

        profileImage.addEventListener('mouseout', function () {
            this.style.transform = 'scale(1) rotate(0deg)';
            this.style.boxShadow = '0 8px 25px rgba(0,0,0,0.2)';
        });
    }

    // Add animated counters for numeric values
    function animateValue(element, start, end, duration) {
        if (start === end) return;
        const range = end - start;
        const increment = end > start ? 1 : -1;
        const stepTime = Math.abs(Math.floor(duration / range));
        let current = start;
        const timer = setInterval(() => {
            current += increment;
            element.textContent = current;
            if (current === end) {
                clearInterval(timer);
            }
        }, stepTime);
    }

    // Add a subtle parallax effect
    window.addEventListener('scroll', () => {
        const scrolled = window.pageYOffset;
        const parallaxElements = document.querySelectorAll('.profile-section');

        parallaxElements.forEach(element => {
            const speed = 0.5;
            element.style.transform = `translateY(${scrolled * speed}px)`;
        });
    });

    // Add notification system
    function showNotification(message, type = 'info') {
        const notification = document.createElement('div');
        notification.className = `notification ${type}`;
        notification.textContent = message;
        document.body.appendChild(notification);

        // Animate notification
        setTimeout(() => {
            notification.style.opacity = '1';
            notification.style.transform = 'translateY(0)';
        }, 100);

        // Remove notification
        setTimeout(() => {
            notification.style.opacity = '0';
            notification.style.transform = 'translateY(-100%)';
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }

    // Add interaction tracking
    let interactionCount = 0;
    document.querySelectorAll('.profile-section').forEach(section => {
        section.addEventListener('click', () => {
            interactionCount++;
            if (interactionCount === 5) {
                showNotification('You\'re exploring quite a bit! Need any help?', 'info');
            }
        });
    });

    // Add dynamic content loading simulation
    function simulateContentLoad() {
        const loadingOverlay = document.createElement('div');
        loadingOverlay.className = 'loading-overlay';
        loadingOverlay.innerHTML = '<div class="loader"></div>';
        document.body.appendChild(loadingOverlay);

        setTimeout(() => {
            loadingOverlay.style.opacity = '0';
            setTimeout(() => loadingOverlay.remove(), 500);
        }, 1000);
    }

    // Initialize dynamic content loading
    simulateContentLoad();

    // Add keyboard navigation
    document.addEventListener('keydown', (e) => {
        const sections = document.querySelectorAll('.profile-section');
        const currentFocus = document.activeElement;

        if (e.key === 'Tab') {
            e.preventDefault();
            const currentIndex = Array.from(sections).indexOf(currentFocus);
            const nextIndex = (currentIndex + 1) % sections.length;
            sections[nextIndex].focus();
        }
    });

    // Add scroll to top button
    const scrollButton = document.createElement('button');
    scrollButton.className = 'scroll-top';
    scrollButton.innerHTML = '↑';
    document.body.appendChild(scrollButton);

    scrollButton.addEventListener('click', () => {
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
    });

    window.addEventListener('scroll', () => {
        if (window.pageYOffset > 300) {
            scrollButton.style.opacity = '1';
            scrollButton.style.transform = 'translateY(0)';
        } else {
            scrollButton.style.opacity = '0';
            scrollButton.style.transform = 'translateY(20px)';
        }
    });
});