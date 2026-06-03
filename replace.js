const fs = require('fs');
const path = require('path');

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        file = path.join(dir, file);
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory() && !file.includes('node_modules') && !file.includes('.git')) {
            results = results.concat(walk(file));
        } else {
            if (file.endsWith('.vue') || file.endsWith('.css') || file.endsWith('.js') || file.endsWith('.ts')) {
                results.push(file);
            }
        }
    });
    return results;
}

const files = walk('e:/Quanly_cafe/FontEnd');
files.forEach(file => {
    let content = fs.readFileSync(file, 'utf8');
    let newContent = content;

    newContent = newContent.replace(/'Montserrat', system-ui, sans-serif/g, "'Inter', sans-serif");
    newContent = newContent.replace(/'DM Sans', system-ui, sans-serif/g, "'Inter', sans-serif");
    newContent = newContent.replace(/"DM Sans"/g, '"Inter"');
    newContent = newContent.replace(/'DM Sans'/g, "'Inter'");
    newContent = newContent.replace(/font-family: 'Montserrat'/g, "font-family: 'Inter'");
    newContent = newContent.replace(/family=Playfair\+Display:wght@500;600;700&family=DM\+Sans:wght@400;500;600;700&display=swap/g, "family=Inter:wght@400;500;600;700;800&family=Playfair+Display:wght@500;600;700&display=swap");

    if (content !== newContent) {
        fs.writeFileSync(file, newContent, 'utf8');
        console.log('Updated', file);
    }
});
