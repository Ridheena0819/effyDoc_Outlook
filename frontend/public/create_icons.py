from PIL import Image, ImageDraw, ImageFont
import os

def create_icon(size, filename):
    # Create a new image with a blue background
    img = Image.new('RGB', (size, size), color='#3b82f6')
    draw = ImageDraw.Draw(img)
    
    # Draw a document icon
    margin = size // 8
    doc_width = size - 2 * margin
    doc_height = size - 2 * margin
    
    # Main document rectangle
    draw.rectangle([margin, margin, size - margin, size - margin], 
                   fill='white', outline='#1e40af', width=2)
    
    # Draw lines to represent text
    line_height = size // 12
    for i in range(3):
        y = margin + (i + 1) * line_height + margin
        draw.rectangle([margin + line_height, y, 
                       size - margin - line_height, y + 2], 
                       fill='#3b82f6')
    
    # Add "D" letter in the center
    try:
        font_size = size // 3
        font = ImageFont.load_default()
        text = "D"
        bbox = draw.textbbox((0, 0), text, font=font)
        text_width = bbox[2] - bbox[0]
        text_height = bbox[3] - bbox[1]
        x = (size - text_width) // 2
        y = (size - text_height) // 2 + size // 8
        draw.text((x, y), text, fill='#1e40af', font=font)
    except:
        pass
    
    img.save(filename, 'PNG')
    print(f"Created {filename} ({size}x{size})")

# Create both icons
create_icon(32, 'icon-32.png')
create_icon(64, 'icon-64.png')
