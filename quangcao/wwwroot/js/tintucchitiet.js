// Calculate reading time
document.addEventListener("DOMContentLoaded", () => {
    // Calculate reading time
    const calculateReadingTime = () => {
        const content = document.querySelector(".article-content")
        if (content) {
            const text = content.textContent || ""
            const wordCount = text.split(/\s+/).length
            const readingTime = Math.ceil(wordCount / 200) // Average reading speed: 200 words per minute
            const readingTimeElement = document.getElementById("reading-time")
            if (readingTimeElement) {
                readingTimeElement.textContent = `${readingTime} phút đọc`
            }
        }
    }

    // Reading progress bar
    const setupReadingProgress = () => {
        const progressBar = document.getElementById("readingProgressBar")
        if (progressBar) {
            window.addEventListener("scroll", () => {
                const winScroll = document.body.scrollTop || document.documentElement.scrollTop
                const height = document.documentElement.scrollHeight - document.documentElement.clientHeight
                const scrolled = (winScroll / height) * 100
                progressBar.style.width = scrolled + "%"
            })
        }
    }

    // Reader mode functionality
    const setupReaderMode = () => {
        const readerModeButton = document.getElementById("readerModeButton")
        const closeReaderMode = document.getElementById("closeReaderMode")
        const readerModeOverlay = document.getElementById("readerModeOverlay")
        const readerModeContent = document.querySelector(".reader-mode-content")
        const articleContent = document.querySelector(".article-content")

        if (readerModeButton && closeReaderMode && readerModeOverlay && readerModeContent && articleContent) {
            readerModeButton.addEventListener("click", () => {
                readerModeContent.innerHTML = articleContent.innerHTML
                readerModeOverlay.style.display = "block"
                setTimeout(() => {
                    readerModeOverlay.style.opacity = "1"
                }, 10)
                document.body.style.overflow = "hidden"
            })

            closeReaderMode.addEventListener("click", () => {
                readerModeOverlay.style.opacity = "0"
                setTimeout(() => {
                    readerModeOverlay.style.display = "none"
                    document.body.style.overflow = "auto"
                }, 300)
            })
        }
    }

    // Print functionality
    const setupPrintButton = () => {
        const printButton = document.getElementById("printButton")
        if (printButton) {
            printButton.addEventListener("click", () => {
                window.print()
            })
        }
    }

    // Initialize all functions
    calculateReadingTime()
    setupReadingProgress()
    setupReaderMode()
    setupPrintButton()

    // Add image lightbox effect
    const setupImageLightbox = () => {
        const articleImages = document.querySelectorAll(".article-content img")
        articleImages.forEach((img) => {
            img.addEventListener("click", () => {
                const lightbox = document.createElement("div")
                lightbox.className = "image-lightbox"
                lightbox.style.position = "fixed"
                lightbox.style.top = "0"
                lightbox.style.left = "0"
                lightbox.style.width = "100%"
                lightbox.style.height = "100%"
                lightbox.style.backgroundColor = "rgba(0, 0, 0, 0.9)"
                lightbox.style.display = "flex"
                lightbox.style.alignItems = "center"
                lightbox.style.justifyContent = "center"
                lightbox.style.zIndex = "10000"
                lightbox.style.opacity = "0"
                lightbox.style.transition = "opacity 0.3s ease"

                const imgClone = document.createElement("img")
                imgClone.src = img.src
                imgClone.alt = img.alt
                imgClone.style.maxWidth = "90%"
                imgClone.style.maxHeight = "90%"
                imgClone.style.borderRadius = "0.5rem"
                imgClone.style.boxShadow = "0 10px 25px rgba(0, 0, 0, 0.3)"

                lightbox.appendChild(imgClone)
                document.body.appendChild(lightbox)

                setTimeout(() => {
                    lightbox.style.opacity = "1"
                }, 10)

                lightbox.addEventListener("click", () => {
                    lightbox.style.opacity = "0"
                    setTimeout(() => {
                        document.body.removeChild(lightbox)
                    }, 300)
                })
            })

            // Add cursor pointer to indicate clickable
            img.style.cursor = "pointer"
        })
    }

    setupImageLightbox()
})

// Social sharing functionality
function shareOnFacebook() {
    const url = encodeURIComponent(window.location.href)
    const title = encodeURIComponent(document.title)
    window.open(`https://www.facebook.com/sharer/sharer.php?u=${url}&t=${title}`, "_blank")
}

function shareOnTwitter() {
    const url = encodeURIComponent(window.location.href)
    const title = encodeURIComponent(document.title)
    window.open(`https://twitter.com/intent/tweet?url=${url}&text=${title}`, "_blank")
}

function shareOnLinkedIn() {
    const url = encodeURIComponent(window.location.href)
    window.open(`https://www.linkedin.com/sharing/share-offsite/?url=${url}`, "_blank")
}

function shareByEmail() {
    const url = encodeURIComponent(window.location.href)
    const title = encodeURIComponent(document.title)
    window.location.href = `mailto:?subject=${title}&body=${url}`
}
