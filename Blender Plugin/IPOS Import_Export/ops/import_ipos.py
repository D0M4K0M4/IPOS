import bpy
from .rotation_converter import import_convert_rotation

class ImportIPOS(bpy.types.Operator):
    bl_idname = "import_scene.ipos"
    bl_label = "Import IPOS"
    bl_options = {'PRESET'}

    filepath: bpy.props.StringProperty(subtype="FILE_PATH")

    def execute(self, context):
        try:
            import_objects_from_file(self.filepath)
        except Exception as e:
            self.report({'ERROR'}, f"Failed to import: {e}")
            return {'CANCELLED'}
        return {'FINISHED'}

    def invoke(self, context, event):
        context.window_manager.fileselect_add(self)
        return {'RUNNING_MODAL'}

def import_objects_from_file(file_path):
    with open(file_path, 'r') as infile:
        lines = infile.readlines()

    current_object = None

    for line in lines:
        line = line.strip()
        if line == "end":
            current_object = None
        elif current_object is None:
            current_object = line
            if current_object not in bpy.data.objects:
                current_object = None
                continue
            obj = bpy.data.objects[current_object]
            obj_rotation_mod = obj.rotation_mode
        else:
            coords = list(map(float, line.split(', ')))
            if len(coords) == 3:
                obj.location = coords
            elif len(coords) == 4:
                if obj_rotation_mod != "QUATERNION":
                    obj.rotation_euler = import_convert_rotation([coords[3], coords[0], coords[1], coords[2]], obj_rotation_mod)
                else:
                    obj.rotation_quaternion = [coords[3], coords[0], coords[1], coords[2]]
