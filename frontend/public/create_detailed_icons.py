from PIL import Image, ImageDraw, ImageFont
import os

def create_detailed_icon(size, filename):
    # Create a new image with white background
    img = Image.new('RGB', (size, size), color=(255, 255, 255))
    draw = ImageDraw.Draw(img)
    
    # Colors
    primary_blue = (37, 99, 235)  # #2563eb
    secondary_blue = (59, 130, 246)  # #3b82f6
    dark_blue = (30, 64, 175)  # #1e40af
    light_blue = (147, 197, 253)  # #93c5fd
    white = (255, 255, 255)
    gray = (107, 114, 128)  # #6b7280
    
    # Draw gradient background
    for y in range(size):
        ratio = y / size
        r = int(primary_blue[0] * (1 - ratio) + secondary_blue[0] * ratio)
        g = int(primary_blue[1] * (1 - ratio) + secondary_blue[1] * ratio)
        b = int(primary_blue[2] * (1 - ratio) + secondary_blue[2] * ratio)
        draw.line([(0, y), (size, y)], fill=(r, g, b))
    
    # Add a border
    border_width = max(1, size // 32)
    draw.rectangle([0, 0, size-1, size-1], outline=dark_blue, width=border_width)
    
    # Document shape
    margin = size // 8
    doc_width = size - 2 * margin
    doc_height = int(doc_width * 1.2)
    
    # Center the document
    doc_x = margin
    doc_y = (size - doc_height) // 2
    
    # Draw document shadow
    shadow_offset = max(1, size // 40)
    draw.rounded_rectangle([doc_x + shadow_offset, doc_y + shadow_offset, 
                           doc_x + doc_width + shadow_offset, doc_y + doc_height + shadow_offset], 
                          radius=size // 20, fill=(0, 0, 0, 100))
    
    # Draw document background
    draw.rounded_rectangle([doc_x, doc_y, doc_x + doc_width, doc_y + doc_height], 
                          radius=size // 20, fill=white, outline=dark_blue, width=max(1, size//20))
    
    # Draw fold corner (top-right)
    fold_size = size // 8
    fold_x = doc_x + doc_width - fold_size
    fold_y = doc_y
    
    # Draw fold triangle
    draw.polygon([(fold_x, fold_y), (doc_x + doc_width, fold_y), 
                  (doc_x + doc_width, fold_y + fold_size)], 
                 fill=light_blue, outline=dark_blue, width=1)
    draw.line([(fold_x, fold_y), (doc_x + doc_width, fold_y + fold_size)], 
             fill=dark_blue, width=1)
    
    # Draw text lines
    line_spacing = max(2, size // 12)
    line_height = max(2, size // 24)
    line_margin = max(3, size // 12)
    
    # Title line (wider)
    title_y = doc_y + line_margin
    draw.rounded_rectangle([doc_x + line_margin, title_y, 
                           doc_x + doc_width - line_margin - fold_size, title_y + line_height + 1], 
                          radius=1, fill=primary_blue)
    
    # Content lines
    for i in range(3):
        y = title_y + (line_height + 2) + line_margin + i * line_spacing
        line_width = doc_width - 2 * line_margin
        if i == 2:  # Make last line shorter
            line_width = int(line_width * 0.6)
        
        draw.rounded_rectangle([doc_x + line_margin, y, 
                               doc_x + line_margin + line_width, y + line_height], 
                              radius=1, fill=gray)
    
    # Add tracking indicator (eye icon)
    if size >= 24:
        eye_size = max(4, size // 6)
        eye_x = doc_x + doc_width - eye_size - line_margin
        eye_y = doc_y + doc_height - eye_size - line_margin
        
        # Eye background circle
        draw.ellipse([eye_x - 2, eye_y - 1, eye_x + eye_size + 2, eye_y + eye_size + 1], 
                    fill=primary_blue)
        
        # Eye shape
        draw.ellipse([eye_x, eye_y, eye_x + eye_size, eye_y + eye_size//2 + 2], 
                    fill=white, outline=dark_blue, width=1)
        
        # Pupil
        pupil_size = max(1, eye_size // 3)
        pupil_x = eye_x + eye_size//2 - pupil_size//2
        pupil_y = eye_y + eye_size//4
        draw.ellipse([pupil_x, pupil_y, pupil_x + pupil_size, pupil_y + pupil_size], 
                    fill=dark_blue)
    
    # Add "effyDOC" text if size is large enough
    if size >= 48:
        try:
            # Try to use a better font if available
            font_size = max(6, size // 8)
            font = ImageFont.load_default()
            text = "eD"
            
            # Get text dimensions
            bbox = draw.textbbox((0, 0), text, font=font)
            text_width = bbox[2] - bbox[0]
            text_height = bbox[3] - bbox[1]
            
            # Position at bottom
            text_x = (size - text_width) // 2
            text_y = size - text_height - margin//2
            
            # Text shadow
            draw.text((text_x + 1, text_y + 1), text, fill=(0, 0, 0, 80), font=font)
            # Main text
            draw.text((text_x, text_y), text, fill=white, font=font)
        except:
            pass
    
    img.save(filename, 'PNG', optimize=True)
    file_size = os.path.getsize(filename)
    print(f"Created detailed {filename} ({size}x{size}), file size: {file_size} bytes")
    return img

# Create icons
create_detailed_icon(16, 'icon-16.png')
create_detailed_icon(32, 'icon-32.png')
create_detailed_icon(64, 'icon-64.png')
create_detailed_icon(128, 'icon-128.png')  # High res version
