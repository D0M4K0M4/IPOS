import bpy
from .rotation_converter import export_convert_rotation

class ExportIPOS(bpy.types.Operator):
    bl_idname = "export_scene.ipos"
    bl_label = "Export IPOS"
    bl_options = {'PRESET'}

    filepath: bpy.props.StringProperty(subtype="FILE_PATH")

    def execute(self, context):
        try:
            # Export only selected objects
            if not bpy.context.selected_objects:
                self.report({'WARNING'}, "No objects selected for position export.")
                return {'CANCELLED'}
            export_selected_objects(self.filepath)
        except Exception as e:
            self.report({'ERROR'}, f"Failed to export: {e}")
            return {'CANCELLED'}
        
        # Report success message
        self.report({'INFO'}, f"File successfully exported to: {self.filepath}")
        return {'FINISHED'}

    def invoke(self, context, event):
        self.filepath = bpy.path.ensure_ext(self.filepath, ".ipos")
        context.window_manager.fileselect_add(self)
        return {'RUNNING_MODAL'}

def export_selected_objects(file_path):
    selected_objects = bpy.context.selected_objects
    lines = []

    for obj in selected_objects:
        if obj.type == 'MESH':  
            lines.append(f"{obj.name}")
            # GTA SA limit the Coordinates to seven decimals as well...
            location = ", ".join(f"{x:.7f}" for x in obj.location)
            obj_rotation_mod = obj.rotation_mode

            if obj_rotation_mod != "QUATERNION":
                rotation = export_convert_rotation(obj.rotation_euler, obj_rotation_mod)
            else:
                # At quaternion it's 10...
                rotation = ", ".join(f"{x:.10f}" for x in [obj.rotation_quaternion[1], obj.rotation_quaternion[2], obj.rotation_quaternion[3], obj.rotation_quaternion[0]])

            lines.append(location)
            lines.append(rotation)
            lines.append("end")

    with open(file_path, 'w') as outfile:
        outfile.write("\n".join(lines))
