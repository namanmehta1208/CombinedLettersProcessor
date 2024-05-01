import random
import os

# Directory where the files will be saved
directory = 'D:\Jobs\CombinedLetters\Input'

# Generate 10 random 8-digit numbers
random_numbers = [random.randint(10000000, 99999999) for _ in range(10)]

# Create files with names following the random ID(8 digits)
for num in random_numbers:
    # File paths for admission and scholarship files
    admission_file_path = os.path.join(directory, f"admission-{num}.txt")
    scholarship_file_path = os.path.join(directory, f"scholarship-{num}.txt")
    
    # Writing "This is admission letter." to admission files
    with open(admission_file_path, 'w') as admission_file:
        admission_file.write("This is admission letter.")
    
    # Writing "This is scholarship letter" to scholarship files
    with open(scholarship_file_path, 'w') as scholarship_file:
        scholarship_file.write("This is scholarship letter.")

print("Files created successfully.")
