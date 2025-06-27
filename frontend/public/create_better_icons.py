from PIL import Image, ImageDraw, ImageFont
import os

def create_professional_icon(size, filename):
    # Create a new image with transparent background
    img = Image.new('RGBA', (size, size), color=(0, 0, 0, 0))
    draw = ImageDraw.Draw(img)
    
    # Colors
    blue = (59, 130, 246, 255)  # #3b82f6
    dark_blue = (30, 64, 175, 255)  # #1e40af
    white = (255, 255, 255, 255)
    
    # Create a rounded rectangle background
    margin = size // 10
    corner_radius = size // 8
    
    # Draw rounded rectangle background
    draw.rounded_rectangle([margin, margin, size - margin, size - margin], 
                          radius=corner_radius, fill=blue, outline=dark_blue, width=2)
    
    # Document icon elements
    doc_margin = size // 4
    doc_width = size - 2 * doc_margin
    doc_height = int(doc_width * 1.3)
    
    # Adjust positioning to center the document
    doc_x = doc_margin
    doc_y = (size - doc_height) // 2
    
    # Draw document background
    draw.rounded_rectangle([doc_x, doc_y, doc_x + doc_width, doc_y + doc_height], 
                          radius=size // 20, fill=white, outline=dark_blue, width=max(1, size//32))
    
    # Draw document lines (text representation)
    line_spacing = max(2, size // 16)
    line_height = max(1, size // 40)
    line_margin = max(2, size // 16)
    
    for i in range(3):
        y = doc_y + line_margin + i * (line_height + line_spacing)
        line_width = doc_width - 2 * line_margin
        if i == 2:  # Make last line shorter
            line_width = int(line_width * 0.7)
        
        draw.rectangle([doc_x + line_margin, y, 
                       doc_x + line_margin + line_width, y + line_height], 
                       fill=blue)
    
    # Add a small "track" indicator (eye icon representation)
    if size >= 32:
        eye_size = size // 8
        eye_x = size - margin - eye_size - 2
        eye_y = margin + 2
        
        # Draw simple eye shape
        draw.ellipse([eye_x, eye_y, eye_x + eye_size, eye_y + eye_size//2], 
                    fill=white, outline=dark_blue, width=1)
        
        # Eye center
        center_size = max(1, eye_size // 4)
        center_x = eye_x + eye_size//2 - center_size//2
        center_y = eye_y + eye_size//4 - center_size//2
        draw.ellipse([center_x, center_y, center_x + center_size, center_y + center_size], 
                    fill=dark_blue)
    
    img.save(filename, 'PNG')
    print(f"Created professional {filename} ({size}x{size}), file size: {os.path.getsize(filename)} bytes")
    return img

# Create both icons
create_professional_icon(32, 'icon-32.png')
create_professional_icon(64, 'icon-64.png')

# Also create 16x16 for completeness
create_professional_icon(16, 'icon-16.png')
