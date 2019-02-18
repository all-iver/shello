const glob = require("glob");
const path = require("path");
const camelCase = require("lodash/camelCase");

const imports = [];

// options is optional
glob("./assets/images/**/*", {}, (er, files) => {
  files.forEach(file => {
    if (path.extname(file) === ".png") {
      const importName = camelCase(path.basename(file, ".png"));

      imports.push(importName);

      console.log(`import ${importName} from "${path.join("../../", file)}"`);
    } else {
      console.log(`Asset not included for preloading: ${file}`);
    }
  });

  console.log("\n");

  imports.forEach(importName => {
    console.log(`${importName},`);
  });
});
