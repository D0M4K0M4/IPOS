import bpy
from .ops.export_ipos import ExportIPOS, export_selected_objects
from .ops.import_ipos import ImportIPOS, import_objects_from_file

bl_info = {
    "name": "IPOS Import-Export",
    "blender": (2, 80, 0),
    "category": "Import-Export",
    "description": "Import and Export objects coordinates and rotation in IPOS format (.ipos) for GTA SA.",
    "location": "File > Import-Export",
}

def menu_func_export(self, context):
    self.layout.operator(ExportIPOS.bl_idname, text="Item position (.ipos)")

def menu_func_import(self, context):
    self.layout.operator(ImportIPOS.bl_idname, text="Item position (.ipos)")

def register():
    bpy.utils.register_class(ExportIPOS)
    bpy.utils.register_class(ImportIPOS)
    bpy.types.TOPBAR_MT_file_export.append(menu_func_export)
    bpy.types.TOPBAR_MT_file_import.append(menu_func_import)

def unregister():
    bpy.utils.unregister_class(ExportIPOS)
    bpy.utils.unregister_class(ImportIPOS)
    bpy.types.TOPBAR_MT_file_export.remove(menu_func_export)
    bpy.types.TOPBAR_MT_file_import.remove(menu_func_import)

if __name__ == "__main__":
    register()
