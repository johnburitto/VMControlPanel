/*
THIS IS A GENERATED/BUNDLED FILE BY ESBUILD
if you want to view the source, please visit the github repository of this plugin
*/

var __defProp = Object.defineProperty;
var __getOwnPropDesc = Object.getOwnPropertyDescriptor;
var __getOwnPropNames = Object.getOwnPropertyNames;
var __hasOwnProp = Object.prototype.hasOwnProperty;
var __export = (target, all) => {
  for (var name in all)
    __defProp(target, name, { get: all[name], enumerable: true });
};
var __copyProps = (to, from, except, desc) => {
  if (from && typeof from === "object" || typeof from === "function") {
    for (let key of __getOwnPropNames(from))
      if (!__hasOwnProp.call(to, key) && key !== except)
        __defProp(to, key, { get: () => from[key], enumerable: !(desc = __getOwnPropDesc(from, key)) || desc.enumerable });
  }
  return to;
};
var __toCommonJS = (mod) => __copyProps(__defProp({}, "__esModule", { value: true }), mod);

// main.ts
var main_exports = {};
__export(main_exports, {
  default: () => ObsidianNiftyLinksPlugin
});
module.exports = __toCommonJS(main_exports);
var import_obsidian = require("obsidian");
var DEFAULT_SETTINGS = {};
var ObsidianNiftyLinksPlugin = class extends import_obsidian.Plugin {
  async onload() {
    console.log("loading plugin");
    await this.loadSettings();
    this.addRibbonIcon("link", "Nifty Links", () => {
      let activeView = this.app.workspace.getActiveViewOfType(import_obsidian.MarkdownView);
      if (activeView) {
        let editor = activeView.editor;
        this.urlToMarkdown(editor);
      }
    });
    this.addCommand({
      id: "create-nifty-links",
      name: "Create Nifty Link",
      editorCheckCallback: (checking, editor) => {
        if (!checking) {
          this.urlToMarkdown(editor);
        }
        return true;
      }
    });
    this.registerMarkdownCodeBlockProcessor("NiftyLinks", (source, el, ctx) => {
      const data = source.split("\n").reduce((acc, line) => {
        const [key, ...value] = line.split(": ");
        acc[key.trim()] = value.join(": ").trim();
        return acc;
      }, {});
      const url = data.url;
      let title = data.title || "";
      let description = data.description || "";
      const imageLink = data.image;
      const iconLink = data.favicon;
      title = title.replace(/\s{3,}/g, " ").trim();
      description = description.replace(/\s{3,}/g, " ").trim();
      const cardTextStyle = imageLink ? "" : ' style="width: 100%;"';
      const iconHTML = iconLink ? `<img class="nifty-link-icon" src="${iconLink}">` : "";
      const imageContainerHTML = imageLink ? `
		  <div class="nifty-link-image-container">
			<div class="nifty-link-image" style="background-image: url('${imageLink}')"></div>
		  </div>` : "";
      const html = `
		  <div class="nifty-link-card-container">
			<a class="nifty-link-card" href="${url}" target="_blank">
			  <div class="nifty-link-card-text"${cardTextStyle}>
				<div class="nifty-link-card-title">${title}</div>
				<div class="nifty-link-card-description">${description}</div>
				<div class="nifty-link-href">
				  ${iconHTML}${url}
				</div>
			  </div>
			  ${imageContainerHTML}
			</a>
		  </div>
		`;
      el.innerHTML = html;
    });
  }
  onunload() {
    console.log("unloading plugin");
  }
  isUrl(text) {
    const urlRegex = new RegExp("^(http:\\/\\/www\\.|https:\\/\\/www\\.|http:\\/\\/|https:\\/\\/)?[a-z0-9]+([\\-.]{1}[a-z0-9]+)*\\.[a-z]{2,5}(:[0-9]{1,5})?(\\/.*)?$");
    return urlRegex.test(text);
  }
  async urlToMarkdown(editor) {
    let selectedText = editor.somethingSelected() ? editor.getSelection().trim() : false;
    if (selectedText && this.isUrl(selectedText)) {
      const url = selectedText;
      try {
        const response = await (0, import_obsidian.requestUrl)({ url: `http://iframely.server.crestify.com/iframely?url=${url}` });
        const data = response.json;
        let imageLink = data.links.find((value) => value.type.startsWith("image") && value.rel.includes("twitter"));
        imageLink = imageLink ? imageLink.href : "";
        let iconLink = data.links.find((value) => value.type.startsWith("image") && value.rel.includes("icon"));
        iconLink = iconLink ? iconLink.href : "";
        let markdownLink = `
\`\`\`NiftyLinks
url: ${url}
title: ${data.meta.title || ""}
description: ${data.meta.description || ""}
favicon: ${iconLink}
${imageLink ? `image: ${imageLink}` : ""}
\`\`\`
`;
        editor.replaceSelection(markdownLink);
        const cursorPos = editor.getCursor();
        editor.setCursor(cursorPos.line + 1, 0);
      } catch (error) {
        console.error(error);
      }
    } else {
      new import_obsidian.Notice("Select a URL to convert to nifty link.");
    }
  }
  async loadSettings() {
    this.settings = Object.assign({}, DEFAULT_SETTINGS, await this.loadData());
  }
  async saveSettings() {
    await this.saveData(this.settings);
  }
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsibWFpbi50cyJdLAogICJzb3VyY2VzQ29udGVudCI6IFsiaW1wb3J0IHtcclxuXHRBcHAsXHJcblx0RWRpdG9yLFxyXG5cdE1hcmtkb3duVmlldyxcclxuXHROb3RpY2UsXHJcblx0UGx1Z2luLFxyXG5cdHJlcXVlc3RVcmxcclxufSBmcm9tIFwib2JzaWRpYW5cIjtcclxuXHJcbmludGVyZmFjZSBPYnNpZGlhbk5pZnR5TGlua3NQbHVnaW5TZXR0aW5ncyB7IH1cclxuXHJcbmNvbnN0IERFRkFVTFRfU0VUVElOR1M6IE9ic2lkaWFuTmlmdHlMaW5rc1BsdWdpblNldHRpbmdzID0ge307XHJcblxyXG5leHBvcnQgZGVmYXVsdCBjbGFzcyBPYnNpZGlhbk5pZnR5TGlua3NQbHVnaW4gZXh0ZW5kcyBQbHVnaW4ge1xyXG5cdHNldHRpbmdzOiBPYnNpZGlhbk5pZnR5TGlua3NQbHVnaW5TZXR0aW5ncztcclxuXHJcblx0YXN5bmMgb25sb2FkKCkge1xyXG5cdFx0Y29uc29sZS5sb2coXCJsb2FkaW5nIHBsdWdpblwiKTtcclxuXHJcblx0XHRhd2FpdCB0aGlzLmxvYWRTZXR0aW5ncygpO1xyXG5cclxuXHRcdHRoaXMuYWRkUmliYm9uSWNvbihcImxpbmtcIiwgXCJOaWZ0eSBMaW5rc1wiLCAoKSA9PiB7XHJcblx0XHRcdGxldCBhY3RpdmVWaWV3ID0gdGhpcy5hcHAud29ya3NwYWNlLmdldEFjdGl2ZVZpZXdPZlR5cGUoTWFya2Rvd25WaWV3KTtcclxuXHRcdFx0aWYgKGFjdGl2ZVZpZXcpIHtcclxuXHRcdFx0XHRsZXQgZWRpdG9yID0gYWN0aXZlVmlldy5lZGl0b3I7XHJcblx0XHRcdFx0dGhpcy51cmxUb01hcmtkb3duKGVkaXRvcik7XHJcblx0XHRcdH1cclxuXHRcdH0pO1xyXG5cclxuXHRcdHRoaXMuYWRkQ29tbWFuZCh7XHJcblx0XHRcdGlkOiBcImNyZWF0ZS1uaWZ0eS1saW5rc1wiLFxyXG5cdFx0XHRuYW1lOiBcIkNyZWF0ZSBOaWZ0eSBMaW5rXCIsXHJcblx0XHRcdGVkaXRvckNoZWNrQ2FsbGJhY2s6IChjaGVja2luZzogYm9vbGVhbiwgZWRpdG9yOiBFZGl0b3IpID0+IHtcclxuXHRcdFx0XHRpZiAoIWNoZWNraW5nKSB7XHJcblx0XHRcdFx0XHR0aGlzLnVybFRvTWFya2Rvd24oZWRpdG9yKTtcclxuXHRcdFx0XHR9XHJcblx0XHRcdFx0cmV0dXJuIHRydWU7XHJcblx0XHRcdH0sXHJcblx0XHR9KTtcclxuXHJcblx0XHR0aGlzLnJlZ2lzdGVyTWFya2Rvd25Db2RlQmxvY2tQcm9jZXNzb3IoXCJOaWZ0eUxpbmtzXCIsIChzb3VyY2UsIGVsLCBjdHgpID0+IHtcclxuXHRcdFx0Y29uc3QgZGF0YSA9IHNvdXJjZS5zcGxpdCgnXFxuJykucmVkdWNlKChhY2MsIGxpbmUpID0+IHtcclxuXHRcdFx0XHRjb25zdCBba2V5LCAuLi52YWx1ZV0gPSBsaW5lLnNwbGl0KCc6ICcpO1xyXG5cdFx0XHRcdGFjY1trZXkudHJpbSgpXSA9IHZhbHVlLmpvaW4oJzogJykudHJpbSgpO1xyXG5cdFx0XHRcdHJldHVybiBhY2M7XHJcblx0XHRcdH0sIHt9KTtcclxuXHJcblx0XHRcdGNvbnN0IHVybCA9IGRhdGEudXJsO1xyXG5cdFx0XHRsZXQgdGl0bGUgPSBkYXRhLnRpdGxlIHx8IFwiXCI7XHJcblx0XHRcdGxldCBkZXNjcmlwdGlvbiA9IGRhdGEuZGVzY3JpcHRpb24gfHwgXCJcIjtcclxuXHRcdFx0Y29uc3QgaW1hZ2VMaW5rID0gZGF0YS5pbWFnZTtcclxuXHRcdFx0Y29uc3QgaWNvbkxpbmsgPSBkYXRhLmZhdmljb247XHJcblxyXG5cdFx0XHQvLyBcdTRGN0ZcdTc1MjgucmVwbGFjZSgvXFxzezMsfS9nLCAnICcpLnRyaW0oKVx1NTkwNFx1NzQwNnRpdGxlXHU1NDhDZGVzY3JpcHRpb25cclxuXHRcdFx0dGl0bGUgPSB0aXRsZS5yZXBsYWNlKC9cXHN7Myx9L2csICcgJykudHJpbSgpO1xyXG5cdFx0XHRkZXNjcmlwdGlvbiA9IGRlc2NyaXB0aW9uLnJlcGxhY2UoL1xcc3szLH0vZywgJyAnKS50cmltKCk7XHJcblxyXG5cdFx0XHRjb25zdCBjYXJkVGV4dFN0eWxlID0gaW1hZ2VMaW5rID8gXCJcIiA6ICcgc3R5bGU9XCJ3aWR0aDogMTAwJTtcIic7XHJcblxyXG5cdFx0XHQvLyBcdTVGNTNpY29uTGlua1x1NUI1OFx1NTcyOFx1NjVGNlx1NjI0RFx1NjNEMlx1NTE2NVx1NTZGRVx1NjgwN1x1RkYwQ1x1Nzg2RVx1NEZERFx1NEUwRFx1NEYxQVx1NUMxRFx1OEJENVx1NTJBMFx1OEY3RFx1NjcyQVx1NUI5QVx1NEU0OVx1NzY4NFx1NTZGRVx1NjgwN1xyXG5cdFx0XHRjb25zdCBpY29uSFRNTCA9IGljb25MaW5rID8gYDxpbWcgY2xhc3M9XCJuaWZ0eS1saW5rLWljb25cIiBzcmM9XCIke2ljb25MaW5rfVwiPmAgOiAnJztcclxuXHJcblx0XHRcdC8vIFx1Njc4NFx1NUVGQVx1NTZGRVx1NzI0N1x1NUJCOVx1NTY2OFx1NzY4NEhUTUxcdUZGMENcdTU5ODJcdTY3OUNcdTY3MDlpbWFnZUxpbmtcclxuXHRcdFx0Y29uc3QgaW1hZ2VDb250YWluZXJIVE1MID0gaW1hZ2VMaW5rID8gYFxyXG5cdFx0ICA8ZGl2IGNsYXNzPVwibmlmdHktbGluay1pbWFnZS1jb250YWluZXJcIj5cclxuXHRcdFx0PGRpdiBjbGFzcz1cIm5pZnR5LWxpbmstaW1hZ2VcIiBzdHlsZT1cImJhY2tncm91bmQtaW1hZ2U6IHVybCgnJHtpbWFnZUxpbmt9JylcIj48L2Rpdj5cclxuXHRcdCAgPC9kaXY+YCA6ICcnO1xyXG5cclxuXHRcdFx0Ly8gXHU2Nzg0XHU1RUZBXHU2NzAwXHU3RUM4XHU3Njg0SFRNTFx1N0VEM1x1Njc4NFxyXG5cdFx0XHRjb25zdCBodG1sID0gYFxyXG5cdFx0ICA8ZGl2IGNsYXNzPVwibmlmdHktbGluay1jYXJkLWNvbnRhaW5lclwiPlxyXG5cdFx0XHQ8YSBjbGFzcz1cIm5pZnR5LWxpbmstY2FyZFwiIGhyZWY9XCIke3VybH1cIiB0YXJnZXQ9XCJfYmxhbmtcIj5cclxuXHRcdFx0ICA8ZGl2IGNsYXNzPVwibmlmdHktbGluay1jYXJkLXRleHRcIiR7Y2FyZFRleHRTdHlsZX0+XHJcblx0XHRcdFx0PGRpdiBjbGFzcz1cIm5pZnR5LWxpbmstY2FyZC10aXRsZVwiPiR7dGl0bGV9PC9kaXY+XHJcblx0XHRcdFx0PGRpdiBjbGFzcz1cIm5pZnR5LWxpbmstY2FyZC1kZXNjcmlwdGlvblwiPiR7ZGVzY3JpcHRpb259PC9kaXY+XHJcblx0XHRcdFx0PGRpdiBjbGFzcz1cIm5pZnR5LWxpbmstaHJlZlwiPlxyXG5cdFx0XHRcdCAgJHtpY29uSFRNTH0ke3VybH1cclxuXHRcdFx0XHQ8L2Rpdj5cclxuXHRcdFx0ICA8L2Rpdj5cclxuXHRcdFx0ICAke2ltYWdlQ29udGFpbmVySFRNTH1cclxuXHRcdFx0PC9hPlxyXG5cdFx0ICA8L2Rpdj5cclxuXHRcdGA7XHJcblxyXG5cdFx0XHRlbC5pbm5lckhUTUwgPSBodG1sO1xyXG5cdFx0fSk7XHJcblxyXG5cclxuXHR9XHJcblxyXG5cdG9udW5sb2FkKCkge1xyXG5cdFx0Y29uc29sZS5sb2coXCJ1bmxvYWRpbmcgcGx1Z2luXCIpO1xyXG5cdH1cclxuXHJcblx0aXNVcmwodGV4dCkge1xyXG5cdFx0Y29uc3QgdXJsUmVnZXggPSBuZXcgUmVnRXhwKFwiXihodHRwOlxcXFwvXFxcXC93d3dcXFxcLnxodHRwczpcXFxcL1xcXFwvd3d3XFxcXC58aHR0cDpcXFxcL1xcXFwvfGh0dHBzOlxcXFwvXFxcXC8pP1thLXowLTldKyhbXFxcXC0uXXsxfVthLXowLTldKykqXFxcXC5bYS16XXsyLDV9KDpbMC05XXsxLDV9KT8oXFxcXC8uKik/JFwiKTtcclxuXHRcdHJldHVybiB1cmxSZWdleC50ZXN0KHRleHQpO1xyXG5cdH1cclxuXHJcblx0YXN5bmMgdXJsVG9NYXJrZG93bihlZGl0b3IpIHtcclxuXHRcdGxldCBzZWxlY3RlZFRleHQgPSBlZGl0b3Iuc29tZXRoaW5nU2VsZWN0ZWQoKVxyXG5cdFx0XHQ/IGVkaXRvci5nZXRTZWxlY3Rpb24oKS50cmltKClcclxuXHRcdFx0OiBmYWxzZTtcclxuXHRcdGlmIChzZWxlY3RlZFRleHQgJiYgdGhpcy5pc1VybChzZWxlY3RlZFRleHQpKSB7XHJcblx0XHRcdGNvbnN0IHVybCA9IHNlbGVjdGVkVGV4dDtcclxuXHRcdFx0dHJ5IHtcclxuXHRcdFx0XHRjb25zdCByZXNwb25zZSA9IGF3YWl0IHJlcXVlc3RVcmwoeyB1cmw6IGBodHRwOi8vaWZyYW1lbHkuc2VydmVyLmNyZXN0aWZ5LmNvbS9pZnJhbWVseT91cmw9JHt1cmx9YCB9KTtcclxuXHRcdFx0XHRjb25zdCBkYXRhID0gcmVzcG9uc2UuanNvbjtcclxuXHRcdFx0XHRsZXQgaW1hZ2VMaW5rID0gZGF0YS5saW5rcy5maW5kKCh2YWx1ZSkgPT4gdmFsdWUudHlwZS5zdGFydHNXaXRoKFwiaW1hZ2VcIikgJiYgdmFsdWUucmVsLmluY2x1ZGVzKCd0d2l0dGVyJykpO1xyXG5cdFx0XHRcdGltYWdlTGluayA9IGltYWdlTGluayA/IGltYWdlTGluay5ocmVmIDogJyc7XHJcblx0XHRcdFx0bGV0IGljb25MaW5rID0gZGF0YS5saW5rcy5maW5kKCh2YWx1ZSkgPT4gdmFsdWUudHlwZS5zdGFydHNXaXRoKFwiaW1hZ2VcIikgJiYgdmFsdWUucmVsLmluY2x1ZGVzKCdpY29uJykpO1xyXG5cdFx0XHRcdGljb25MaW5rID0gaWNvbkxpbmsgPyBpY29uTGluay5ocmVmIDogJyc7XHJcblxyXG5cdFx0XHRcdC8vIFx1NjgzOVx1NjM2RVx1NjYyRlx1NTQyNlx1NjcwOVx1NTZGRVx1NzI0N1x1OTRGRVx1NjNBNVx1OEMwM1x1NjU3NE1hcmtkb3duXHU4RjkzXHU1MUZBXHJcblx0XHRcdFx0bGV0IG1hcmtkb3duTGluayA9IGBcXG5cXGBcXGBcXGBOaWZ0eUxpbmtzXHJcbnVybDogJHt1cmx9XHJcbnRpdGxlOiAke2RhdGEubWV0YS50aXRsZSB8fCBcIlwifVxyXG5kZXNjcmlwdGlvbjogJHtkYXRhLm1ldGEuZGVzY3JpcHRpb24gfHwgXCJcIn1cclxuZmF2aWNvbjogJHtpY29uTGlua31cclxuJHtpbWFnZUxpbmsgPyBgaW1hZ2U6ICR7aW1hZ2VMaW5rfWAgOiBcIlwifVxyXG5cXGBcXGBcXGBcXG5gO1xyXG5cclxuXHJcblx0XHRcdFx0ZWRpdG9yLnJlcGxhY2VTZWxlY3Rpb24obWFya2Rvd25MaW5rKTtcclxuXHRcdFx0XHRjb25zdCBjdXJzb3JQb3MgPSBlZGl0b3IuZ2V0Q3Vyc29yKCk7XHJcblx0XHRcdFx0ZWRpdG9yLnNldEN1cnNvcihjdXJzb3JQb3MubGluZSArIDEsIDApO1xyXG5cdFx0XHR9IGNhdGNoIChlcnJvcikge1xyXG5cdFx0XHRcdGNvbnNvbGUuZXJyb3IoZXJyb3IpO1xyXG5cdFx0XHR9XHJcblx0XHR9XHJcblx0XHRlbHNlIHtcclxuXHRcdFx0bmV3IE5vdGljZShcIlNlbGVjdCBhIFVSTCB0byBjb252ZXJ0IHRvIG5pZnR5IGxpbmsuXCIpO1xyXG5cdFx0fVxyXG5cdH1cclxuXHJcblxyXG5cdGFzeW5jIGxvYWRTZXR0aW5ncygpIHtcclxuXHRcdHRoaXMuc2V0dGluZ3MgPSBPYmplY3QuYXNzaWduKHt9LCBERUZBVUxUX1NFVFRJTkdTLCBhd2FpdCB0aGlzLmxvYWREYXRhKCkpO1xyXG5cdH1cclxuXHJcblx0YXN5bmMgc2F2ZVNldHRpbmdzKCkge1xyXG5cdFx0YXdhaXQgdGhpcy5zYXZlRGF0YSh0aGlzLnNldHRpbmdzKTtcclxuXHR9XHJcbn1cclxuIl0sCiAgIm1hcHBpbmdzIjogIjs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7O0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBLHNCQU9PO0FBSVAsSUFBTSxtQkFBcUQsQ0FBQztBQUU1RCxJQUFxQiwyQkFBckIsY0FBc0QsdUJBQU87QUFBQSxFQUc1RCxNQUFNLFNBQVM7QUFDZCxZQUFRLElBQUksZ0JBQWdCO0FBRTVCLFVBQU0sS0FBSyxhQUFhO0FBRXhCLFNBQUssY0FBYyxRQUFRLGVBQWUsTUFBTTtBQUMvQyxVQUFJLGFBQWEsS0FBSyxJ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
